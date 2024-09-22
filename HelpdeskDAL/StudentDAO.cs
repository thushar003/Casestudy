using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace ExercisesDAL
{
    public class StudentDAO
    {
        public async Task<Student> GetByLastname(string name)
        {
            Student? selectedStudent;
            try
            {
                SomeSchoolContext _db = new();
                selectedStudent = await _db.Students.FirstOrDefaultAsync(stu => stu.LastName == name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return selectedStudent;
        }

        public async Task<Student> GetById(int id)
        {
            Student? selectedStudent;
            try
            {
                SomeSchoolContext _db = new();
                selectedStudent = await _db.Students.FindAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return selectedStudent;
        }

        public async Task<Student> GetByPhoneNumber(string phoneNum)
        {
            Student? selectedStudent;
            try
            {
                SomeSchoolContext _db = new();
                selectedStudent = await _db.Students.FirstOrDefaultAsync(stu => stu.PhoneNo == phoneNum);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return selectedStudent;
        }

        public async Task<List<Student>> GetAll()
        {
            List<Student> allStudents;
            try
            {
                SomeSchoolContext _db = new();
                allStudents = await _db.Students.ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }
            return allStudents;
        }

        public async Task<int> Add(Student newStudent)
        {
            try
            {
                SomeSchoolContext _db = new();
                await _db.Students.AddAsync(newStudent);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return newStudent.Id;
        }

        public async Task<int> Update(Student updatedStudent)
        {
            int studentUpdated = -1;
            try
            {
                SomeSchoolContext _db = new();
                Student? currentStudent = await _db.Students.FirstOrDefaultAsync(stu => stu.Id == updatedStudent.Id);
                _db.Entry(currentStudent!).CurrentValues.SetValues(updatedStudent);
                studentUpdated = await _db.SaveChangesAsync();      //should return 1
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return studentUpdated;
        }

        public async Task<int> Delete(int? id)
        {
            int studentsDeleted = -1;
            try
            {
                SomeSchoolContext _db = new();
                Student? selectedStudent = await _db.Students.FirstOrDefaultAsync(stu => stu.Id == id);
                _db.Students.Remove(selectedStudent!);
                studentsDeleted = await _db.SaveChangesAsync();     //returns # of rows removed
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem in " + GetType().Name + " " + MethodBase.GetCurrentMethod()!.Name + " " + ex.Message);
                throw;
            }

            return studentsDeleted;
        }
    }
}
