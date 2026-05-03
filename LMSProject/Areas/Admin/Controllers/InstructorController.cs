using AutoMapper;
using LMSProject.Areas.Admin.Helpers;
using LMSProject.Areas.Admin.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MLSCore;
using MLSCore.DTO;
using MLSCore.IdentityModel;
using MLSCore.Models;
using System.ComponentModel.DataAnnotations;

namespace LMSProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InstructorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        UserManager<ApplicationUser> _usermanager;
        SignInManager<ApplicationUser> _signInManager;
        public InstructorController(IMapper mapper,IUnitOfWork unitOfWork,UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signInManager)
        {
            _mapper = mapper;
            _usermanager = usermanager;
            _signInManager = signInManager;
            _unitOfWork= unitOfWork;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new InstructorVM());
        }
        [HttpPost]
        public async Task<IActionResult> Register(InstructorVM model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);
            ApplicationUser user = new ApplicationUser()
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName=model.UserName

            };
            try
           {
                var result = await _usermanager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _usermanager.AddToRoleAsync(user, "Instructor");
                    if (model.Image != null)
                    {
                        string folder = "Images/images/";
                        model.ImageName = Upload.UploadImage(folder, model.Image);
                    }
                    TbInstructor instructor = new TbInstructor()
                    {
                        Bio = model.Bio,
                        FullName = model.FullName,
                        Specialization = model.Specialization,
                        ExperienceYears = model.ExperienceYears,
                        ShowInHomePage = model.ShowInHomePage,
                        UserId = user.Id,
                        Rating = 0,
                        ImageName=model.ImageName
                    };
                    await _unitOfWork.Instructors.AddAsync(instructor);

                    _unitOfWork.Complete();

                    ViewBag.Messag = "Stage Added Successfully";
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
        public async Task<IActionResult> Index()
        {
            IEnumerable<InstructorCoursesDTO> instructors = await _unitOfWork.Instructors.FindAllInstCourses();
            // return Ok(stages);
            return View(instructors);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var InstructorDTO =await _unitOfWork.Instructors.GetInstructorForEdit(Id);
            InstructorEditVM InsVM = _mapper.Map<InstructorEditVM>(InstructorDTO);
            return View(InsVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(InstructorEditVM instructor)
        {
            TbInstructor instructorData = await _unitOfWork.Instructors.GetById(instructor.Id);
            string userid = instructorData.UserId;
            var user = await _usermanager.FindByIdAsync(userid);

            if (user != null)
            {

                // Update fields
                user.UserName = instructor.UserName;
                user.Email = instructor.Email;
                user.PhoneNumber = instructor.PhoneNumber;
                
                var result = await _usermanager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (instructor.Password != null)
                    {
                        var token = await _usermanager.GeneratePasswordResetTokenAsync(user);

                        var result2 = await _usermanager.ResetPasswordAsync(user, token, instructor.Password);
                    }
                }
                if (instructor.Image != null)
                {
                    string prevImage = instructor.ImageName;
                    //--------------------------delete image from folder
                    Upload.DeletImage(prevImage);
                    string folder = "Images/images/";
                    instructor.ImageName = Upload.UploadImage(folder, instructor.Image);
                }

                //-------------------------edit instructor table
                instructorData.Id = instructor.Id;
                instructorData.FullName = instructor.FullName;
                instructorData.Bio = instructor.Bio;
                instructorData.Specialization = instructor.Specialization;
                instructorData.ExperienceYears = instructor.ExperienceYears;
                instructorData.UpdatedBy = "Admin";
                instructorData.UpdatedDate = DateTime.Now;
                instructorData.ImageName = instructor.ImageName;                

                await _unitOfWork.Instructors.Update(instructorData);
                _unitOfWork.Complete();
                ViewBag.Messag = "Stage Updated Successfully";

            }

            //ApplicationUser usr=await _usermanager.FindByIdAsync
            return View(instructor);
           
        }
        private  async Task EditInstructorData(InstructorEditVM instructor)
        {
           
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int Id)

        {
            var instructor = await _unitOfWork.Instructors.GetById(Id);
            //--------------------------delete image from folder
            Upload.DeletImage(instructor.ImageName);

            instructor.CurrentState = 0;
            _unitOfWork.Instructors.Update(instructor);

            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}
