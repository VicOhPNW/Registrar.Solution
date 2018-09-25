using Microsoft.AspNetCore.Mvc;
using Registrar.Models;
using System;
using System.Collections.Generic;

namespace Registrar.Controllers
{
  public class StudentsController : Controller
  {
    [HttpGet("/students")]
    public ActionResult Index()
    {
      List<Student> allStudents = Student.GetAll();
      // List<Course> allCourses = Course.GetAll();
      return View(allStudents);
    }

    [HttpGet("/students/new")]
    public ActionResult CreateForm()
    {
      return View();
    }

    [HttpPost("/students")]
    public ActionResult Create(string name, string date)
    {
      Student newStudent = new Student(name, date);
      newStudent.Save();
      return RedirectToAction("Index");
    }

    [HttpGet("/students/{id}")]
    public ActionResult Details(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Student selectedStudent = Student.Find(id);
      List<Course> studentCourses = selectedStudent.GetCourses();
      List<Course> allCourses = Course.GetAll();

      model.Add("selectedStudent", selectedStudent);
      model.Add("studentCourses", studentCourses);
      model.Add("allCourses", allCourses);

      return View(model);
    }

    [HttpPost("/students/{studentId}/courses/new")]
      public ActionResult AddCourse(int studentId, int courseId)
      {
          Student student = Student.Find(studentId);
          Course course = Course.Find(courseId);
          student.AddCourse(course);
          return RedirectToAction("Details",  new { id = studentId });
      }

    [HttpGet("/students/{id}/done/{courseId}")]
    public ActionResult Done(int id, int courseId)
    {
      Course doneCourse = Course.Find(courseId);
      doneCourse.Done();
      return RedirectToAction("Details", new {id = id});
    }

  }
}
