using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


namespace GameDataLibrary
{
    /// <summary>
    /// Board Games Data Model
    /// </summary>
    [Table("BoardGames")]
    public class BoardGameModel : BaseModel
    {
       
        // Game Description
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Image URL
        /// </summary>
        [Required]
        [StringLength(500)]
        [Url]
        public string ImageURL { get; set; } = null!;

        /// <summary>
        /// Publisher Table key
        /// </summary>
        [Required]
        [ForeignKey("PublisherModel")]
        public int PublishersId { get; set; }

        /// <summary>
        /// Publisher Data
        /// </summary>
        public virtual PublisherModel? Publishers { get; set; }

        /// <summary>
        /// Publisher Name
        /// </summary>
        [NotMapped]
        public string Publisher
        {
            get
            {
                if (Publishers is not null)
                {
                    return Publishers.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Used to build a Json string of this object.
        /// </summary>
        /// <returns>Json String.</returns>
        public override string ToString()
        {
        
            return JsonSerializer.Serialize(this);
        }
    }
}
