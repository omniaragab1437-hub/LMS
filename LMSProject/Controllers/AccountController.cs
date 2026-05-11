using AutoMapper;
using LMSProject.Areas.Admin.Helpers;
using LMSProject.Areas.Admin.ViewModel;
using LMSProject.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MLSCore;
using MLSCore.IdentityModel;
using MLSCore.Models;

namespace LMSProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountController(IMapper mapper, IUnitOfWork unitOfWork,UserManager<ApplicationUser> usermanager,
                                 SignInManager<ApplicationUser> signInManager)
        {
            _usermanager = usermanager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Register()
        {
            List<SelectDropList> GradeList = (await _unitOfWork.Grades.FindAllAsyncDroplist(x => x.CurrentState == 1,
               x => new SelectDropList
               { Id = x.Id, Name = x.Name })).ToList();
            StudentVM stVM = new StudentVM();
            stVM.Grades = GradeList;

            return View(stVM);
        }
        [HttpPost]
        public async Task<IActionResult> Register(StudentVM model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);
            ApplicationUser user = new ApplicationUser()
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName

            };
            try
            {
                var result = await _usermanager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _usermanager.AddToRoleAsync(user, "Student");
                    if (model.Image != null)
                    {
                        string folder = "Images/images/";
                        model.ImageName = Upload.UploadImage(folder, model.Image);
                    }
                    TbStudent student = new TbStudent()
                    {
                       
                        FullName = model.FullName,
                        GradeId = model.GradeId,
                        UserId = user.Id,
                        ParentMobile=model.ParentMobile,
                        ParentNationalId=model.ParentNationalId,
                        ImageName = model.ImageName
                    };
                    await _unitOfWork.Students.AddAsync(student);

                    _unitOfWork.Complete();

                    ViewBag.Messag = "Data Added Successfully";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }
            }
            catch (Exception ex) { }

            return View(model);
        }
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _usermanager.FindByEmailAsync(model.EmailOrUsername);

                if (user == null)
                {
                    user = await _usermanager.FindByNameAsync(model.EmailOrUsername);
                }
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid Login");
                    return View(model);
                }
                
                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe, 
                    false
                );

                if (result.Succeeded)
                {
                    if (await _usermanager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction
                        (
                            "Index",
                            "Home",
                            new { area = "Admin" }
                        );
                    }

                    if (await _usermanager.IsInRoleAsync(user, "Instructor"))
                    {
                        return RedirectToAction
                        (
                            "Index",
                            "Home",
                            new { area = "Instructor" }
                        );
                    }

                    // Default Redirect
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }


            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _usermanager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _usermanager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { token, email = user.Email },
                Request.Scheme);

            // مؤقتاً نعرض اللينك
            ViewBag.ResetLink = resetLink;

            return View("ForgotPasswordConfirmation");
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordVM
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _usermanager.FindByEmailAsync(model.Email);

            if (user == null)
                return RedirectToAction("Login");

            var result = await _usermanager.ResetPasswordAsync(
                user,
                model.Token,
                model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
