using AutoMapper;
using LMSProject.Areas.Admin.Helpers;
using LMSProject.Areas.Admin.ViewModel;
using LMSProject.Areas.Instructor.ViewModel;
using LMSProject.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLSCore;
using MLSCore.IdentityModel;
using MLSCore.Models;

namespace LMSProject.Areas.Instructor.Controllers
{
    [Area("Instructor")]

    public class CourseController : BaseController
    {
        int Instructorid = 1;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CourseController(IUnitOfWork unitOfWork, IMapper mapper,UserManager<ApplicationUser> userManager)
        : base(userManager)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CourseVM Course = new CourseVM();
            //-----------------------------------------subject list-------------------------
            List<SelectDropList> subjList = (await _unitOfWork.Subjects.FindAllAsyncDroplist(x => x.CurrentState == 1,
                x => new SelectDropList
                { Id = x.Id, Name = x.Name })).ToList();
            int FsubjId = subjList[0].Id;
            //-----------------------------------------subsubject list-------------------------
            List<SelectDropList> subsubjList = (await _unitOfWork.SubSubjects.FindAllAsyncDroplist(x => x.CurrentState == 1&&
            x.SubjectId==FsubjId,
                x => new SelectDropList
                { Id = x.Id, Name = x.Name })).ToList();
            //-----------------------------------------Term list-------------------------
            List<SelectDropList> terms = (await _unitOfWork.Terms.FindAllAsyncDroplist(x => x.CurrentState == 1,
                x => new SelectDropList
                { Id = x.Id, Name = x.Name })).ToList();
            
            //-----------------------------------------grade list-------------------------
            List<SelectDropList> grades = (await _unitOfWork.Grades.FindAllAsyncDroplist(x => x.CurrentState == 1,
                x => new SelectDropList
                { Id = x.Id, Name = x.Name })).ToList();
            
            Course.Subjects= subjList;
            Course.SubSubjects = subsubjList;
            Course.Terms = terms;
            Course.Grades = grades;
            ;
            return View(Course);
        }
        [HttpPost]
        public async Task<ActionResult> Create(CourseVM courseVM)
        {

            if (ModelState.IsValid)
            {
                // List<SelectDropList> subList = subsubjectVM.Subjects;
                if (courseVM.Image != null)
                {
                    string folder = "Images/images/";
                    courseVM.ImageName = Upload.UploadImage(folder, courseVM.Image);
                }

                TbCourse course = _mapper.Map<TbCourse>(courseVM);
                //----------------------get instructor ID
               // var userId = CurrentUserId;
              //  var IstructorId


                await _unitOfWork.Courses.AddAsync(course);

                _unitOfWork.Complete();
                ViewBag.Messag = "subsubject Added Successfully";


            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetBySubjectId(int subjectId)
        {
            List<SelectDropList> subList = (await _unitOfWork.SubSubjects.FindAllAsyncDroplist
                (x => x.CurrentState == 1&&x.SubjectId==subjectId,
                x => new SelectDropList
                { Id = x.Id, Name = x.Name })).ToList();

            return Json(subList);
        }
    }
}
