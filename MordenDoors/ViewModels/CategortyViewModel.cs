namespace MordenDoors.ViewModels
{
    public class CategortyViewModel
    {
        public int ID { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public int? Sort { get; set; }

        public bool IsMain { get; set; }
        public bool IsActive { get; set; }
        public bool isUsed { get; set; }
        public CategortyViewModel()
        {
            IsActive = true;

        }
    }
}