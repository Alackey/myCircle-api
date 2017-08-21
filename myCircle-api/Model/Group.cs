using System.ComponentModel.DataAnnotations;

namespace myCircle_api.Model
{
    public class Group
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool PrivateVis { get; set; }
        public bool GroupPage { get; set; }
        public string PhotoUrl { get; set; }
        public string BackgroundPhotoUrl { get; set; }
        public string Description { get; set; }
        public string NotificationsId { get; set; }
        public string EventsId { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public bool OfficialClub { get; set; }
        public bool Discoverable { get; set; }
    }
}