using System.ComponentModel.DataAnnotations;

namespace myCircle_api.Model
{
    public class Category
    {
        [Key] 
        public string Name { get; set; }
    }
}