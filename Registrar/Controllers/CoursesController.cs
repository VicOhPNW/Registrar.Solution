using Microsoft.AspNetCore.Mvc;
using Registrar.Models;
using System;
using System.Collections.Generic;

namespace Registrar.Controllers
{
  public class CoursesController : Controller
  {
    [HttpGet("/courses")]
    public ActionResult Index()
    {
      // List<Course> allCourses = Course.GetAll();
      return View(Course.GetAll());
    }

    [HttpGet("courses/new")]
    public ActionResult CreateForm()
    {
      return View();
    }

    [HttpPost("/courses")]
    public ActionResult Create(string name, string courseNumber)
    {
      Course newCourse = new Course(name, courseNumber);
      newCourse.Save();
      return RedirectToAction("Index");
    }

    [HttpGet("/courses/{id}")]
    public ActionResult Details(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Course selectedCourse = Course.Find(id);
      List<Student> courseStudents = selectedCourse.GetStudents();
      List<Student> allStudents = Student.GetAll();

      model.Add("selectedCourse", selectedCourse);
      model.Add("courseStudents", courseStudents);
      model.Add("allStudents", allStudents);

      return View(model);
    }

    [HttpPost("/courses/{courseId}/students/new")]
      public ActionResult AddStudent(int studentId, int courseId)
      {
          Student student = Student.Find(studentId);
          Course course = Course.Find(courseId);
          course.AddStudent(student);
          return RedirectToAction("Details",  new { id = courseId });
      }

  }
}
