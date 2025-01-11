namespace AuthSystem.Models
{
    public class Department
    {
        // Primary Key
        public int Id { get; set; }

        // Name of the department (required, max length 100)
        public string Name { get; set; }


       // public ICollection<Semester> Semesters { get; set; } // Lista e semestrave


    }
}
