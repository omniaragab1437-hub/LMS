using AutoMapper;
using LMSProject.Areas.Admin.Helpers;
using LMSProject.Areas.Admin.ViewModel;
using LMSProject.Areas.Instructor.ViewModel;
using LMSProject.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MLSCore;
using MLSCore.IdentityModel;
using MLSCore.Models;

namespace LMSProject.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize]
    public class CourseController : BaseController
    {
        //int Instructorid = 1;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CourseController(IUnitOfWork unitOfWork, IMapper mapper,UserManager<ApplicationUser> userManager)
        : base(userManager)
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task< int> GetInstructorId()
        {
            var userId = CurrentUserId;
            TbInstructor instructor = await _unitOfWork.Instructors
.FindAsync(i => i.UserId == userId);
            var instructorId = instructor.Id;
            return instructorId;
        }
        public async Task<IActionResult> Index()
        {
            var instructorId =await GetInstructorId();


            IEnumerable <TbCourse> courses = await _unitOfWork.Courses.FindAllAsync(a => a.CurrentState == 1&&a.InstructorId==instructorId);
            // return Ok(stages);
            return View(courses);

           
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
               
                var instructorId=await GetInstructorId();
                course.InstructorId = instructorId;

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
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var Course = await _unitOfWork.Courses.GetById(Id);
            CourseEditVM courseditVM = _mapper.Map<CourseEditVM>(Course);
            //---------------------------------------subjId---------------------------------
            var subSubject = await _unitOfWork.SubSubjects.FindAsync(
     i => i.Id == Course.SubSubjId );
            var subjectId = subSubject.SubjectId;
            courseditVM.SubjId = subjectId;
            //-----------------------------------------subject list-------------------------
            List<SelectDropList> subjList = (await _unitOfWork.Subjects.FindAllAsyncDroplist(x => x.CurrentState == 1,
                x => new SelectDropList
                { Id = x.Id, Name = x.Name })).ToList();
            
            //-----------------------------------------subsubject list-------------------------
            List<SelectDropList> subsubjList = (await _unitOfWork.SubSubjects.FindAllAsyncDroplist(x => x.CurrentState == 1 &&
            x.SubjectId == subjectId,
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

            courseditVM.Subjects = subjList;
            courseditVM.SubSubjects = subsubjList;
            courseditVM.Terms = terms;
            courseditVM.Grades = grades;
            ;
            return View(courseditVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CourseEditVM courseEditVM)
        {
            if (ModelState.IsValid)
            {
                if (courseEditVM.Image != null)
                {
                    string prevImage = courseEditVM.ImageName;
                    //--------------------------delete image from folder
                    Upload.DeletImage(prevImage);
                    string folder = "Images/images/";
                    courseEditVM.ImageName = Upload.UploadImage(folder, courseEditVM.Image);
                }
                var course = await _unitOfWork.Courses.GetById(courseEditVM.Id);
                _mapper.Map(courseEditVM, course);

                await _unitOfWork.Courses.Update(course);

                _unitOfWork.Complete();
                ViewBag.Messag = "Course Updated Successfully";

            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int Id)

        {
            var course = await _unitOfWork.Courses.GetById(Id);
            //--------------------------delete image from folder
            Upload.DeletImage(course.ImageName);

            course.CurrentState = 0;
            _unitOfWork.Courses.Update(course);

            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

    }
}
