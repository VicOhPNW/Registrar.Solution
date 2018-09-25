using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Registrar;
using System;

namespace Registrar.Models
{
  public class Student
  {
    public int id{get; set; }
    public string name{get; set; }
    public string date {get; set; }
    public Student(string newName, string newDate, int newId = 0)
    {
      name = newName;
      date = newDate;
      id = newId;
    }
    public static List<Student> GetAll()
    {
      List<Student> allStudents = new List<Student> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM students;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        string studentDate = rdr.GetString(2);
        Student newStudent = new Student(studentName, studentDate, studentId);
        allStudents.Add(newStudent);
      }
      conn.Close();
      if (conn !=null)
      {
        conn.Dispose();
      }
      return allStudents;
    }
    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students (name, date) VALUES (@studentName, @studentDate);";

      MySqlParameter studentName = new MySqlParameter();
      studentName.ParameterName = "@studentName";
      studentName.Value = this.name;
      cmd.Parameters.Add(studentName);

      MySqlParameter studentDate = new MySqlParameter();
      studentDate.ParameterName = "@studentDate";
      studentDate.Value = this.date;
      cmd.Parameters.Add(studentDate);

      cmd.ExecuteNonQuery();
      id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn !=null)
      {
        conn.Dispose();
      }
    }
    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
        return false;
      }
      else
      {

        Student newStudent = (Student) otherStudent;
        bool idEquality = (this.id == newStudent.id);
        bool nameEquality = (this.name == newStudent.name);
        bool dateEquality = (this.date == newStudent.date);

        return (nameEquality && idEquality && dateEquality);
      }
    }
    public override int GetHashCode()
    {
      return this.name.GetHashCode();
    }
    public static Student Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM `students` WHERE id = @thisId;";

      MySqlParameter thisId = new MySqlParameter();
      thisId.ParameterName = "@thisId";
      thisId.Value = id;
      cmd.Parameters.Add(thisId);

      var rdr = cmd.ExecuteReader() as MySqlDataReader;

      int studentId = 0;
      string studentName = "";
      string studentDate = "";


      while (rdr.Read())
      {
          studentId = rdr.GetInt32(0);
          studentName = rdr.GetString(1);
          studentDate = rdr.GetString(2);
      }

      Student foundStudent= new Student(studentName, studentDate, studentId);  // This line is new!

       conn.Close();
       if (conn != null)
       {
           conn.Dispose();
       }
      return foundStudent;
    }

    public void AddCourse(Course newCourse)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"INSERT INTO students_courses (student_id, course_id) VALUES (@studentId, @courseId);";

        MySqlParameter student_id = new MySqlParameter();
        student_id.ParameterName = "@studentId";
        student_id.Value = id;
        cmd.Parameters.Add(student_id);

        MySqlParameter course_id = new MySqlParameter();
        course_id.ParameterName = "@courseId";
        course_id.Value = newCourse.id;
        cmd.Parameters.Add(course_id);

        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
      }

    public List<Course> GetCourses()
      {
          MySqlConnection conn = DB.Connection();
          conn.Open();
          MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
          cmd.CommandText = @"SELECT courses.* FROM students
              JOIN students_courses ON (students.id = students_courses.student_id)
              JOIN courses ON (students_courses.course_id = courses.id)
              WHERE students.id = @studentId;";

          MySqlParameter studentId = new MySqlParameter();
          studentId.ParameterName = "@studentId";
          studentId.Value = id;
          cmd.Parameters.Add(studentId);

          MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
          List<Course> courses = new List<Course>{};

          while(rdr.Read())
          {
            int courseId = rdr.GetInt32(0);
            string courseName = rdr.GetString(1);
            string courseNumber = rdr.GetString(2);

            Course newCourse = new Course(courseName, courseNumber, courseId);
            courses.Add(newCourse);
          }
          conn.Close();
          if (conn != null)
          {
              conn.Dispose();
          }
          return courses;
      }
    }
}
