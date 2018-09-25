using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Registrar;
using System;

namespace Registrar.Models
{
  public class Course
  {
    public int id{get; set; }
    public string name{get; set; }
    public string courseNumber{get; set; }
    public bool complete{get; set; }

    public Course(string newName, string newCourseNumber, int newId = 0)
    {
      name = newName;
      courseNumber = newCourseNumber;
      id = newId;
      complete = false;
    }

    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        bool idEquality = (this.id == newCourse.id);
        bool nameEquality = (this.name == newCourse.name);
        bool courseNumberEquality = (this.courseNumber == newCourse.courseNumber);

        return (courseNumberEquality && nameEquality && idEquality);
      }
    }

    public override int GetHashCode()
    {
      return this.name.GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO courses (course_name, course_number) VALUES (@courseName, @courseNumber);";

      MySqlParameter courseName = new MySqlParameter();
      courseName.ParameterName = "@courseName";
      courseName.Value = this.name;
      cmd.Parameters.Add(courseName);

      MySqlParameter courseNumber = new MySqlParameter();
      courseNumber.ParameterName = "@courseNumber";
      courseNumber.Value = this.courseNumber;
      cmd.Parameters.Add(courseNumber);

      cmd.ExecuteNonQuery();
      id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Course> GetAll()
    {
      List<Course> allCourses = new List<Course> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM courses;";

      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);
        Course newCourse = new Course(courseName, courseNumber, courseId);
        allCourses.Add(newCourse);
      }
      conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
      return allCourses;
    }
    public static Course Find(int id)
    {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT * FROM courses WHERE id = (@searchId);";

        MySqlParameter searchId = new MySqlParameter();
        searchId.ParameterName = "@searchId";
        searchId.Value = id;
        cmd.Parameters.Add(searchId);

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        int courseId = 0;
        string courseName = "";
        string courseNumber = "";

        while(rdr.Read())
        {
          courseId = rdr.GetInt32(0);
          courseName = rdr.GetString(1);
          courseNumber = rdr.GetString(2);
        }
        Course newCourse = new Course(courseName, courseNumber, courseId);
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return newCourse;
    }

    public void AddStudent(Student newStudent)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students_courses (student_id, course_id) VALUES (@studentId, @courseId);";

      MySqlParameter student_id = new MySqlParameter();
      student_id.ParameterName = "@studentId";
      student_id.Value = newStudent.id;
      cmd.Parameters.Add(student_id);

      MySqlParameter course_id = new MySqlParameter();
      course_id.ParameterName = "@courseId";
      course_id.Value = id;
      cmd.Parameters.Add(course_id);

      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
          conn.Dispose();
      }
    }

      public List<Student> GetStudents()
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();
        MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"SELECT students.* FROM courses
            JOIN students_courses ON (courses.id = students_courses.course_id)
            JOIN students ON (students_courses.student_id = students.id)
            WHERE courses.id = @courseId;";

        MySqlParameter courseId = new MySqlParameter();
        courseId.ParameterName = "@courseId";
        courseId.Value = id;
        cmd.Parameters.Add(courseId);

        MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
        List<Student> students = new List<Student>{};

        while(rdr.Read())
        {
          int studentId = rdr.GetInt32(0);
          string studentName = rdr.GetString(1);
          string studentDate = rdr.GetString(2);

          Student newStudent = new Student(studentName, studentDate, studentId);
          students.Add(newStudent);
        }
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }
        return students;
      }
      public void Done()
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"UPDATE courses SET status = @status WHERE id = @courseId;";

        MySqlParameter statusParameter = new MySqlParameter();
        statusParameter.ParameterName = "@status";
        statusParameter.Value = true;
        cmd.Parameters.Add(statusParameter);
        // cmd.Parameters.AddWithValue("@status", true);

        MySqlParameter courseIdParameter = new MySqlParameter();
        // courseIdParameter.ParameterName = "@courseId";
        // courseIdParameter.Value = id;
        cmd.Parameters.AddWithValue("@courseIdParameter", id);

        cmd.ExecuteNonQuery();
        conn.Close();
        if (conn != null)
        {
            conn.Dispose();
        }

      }
  }
}
