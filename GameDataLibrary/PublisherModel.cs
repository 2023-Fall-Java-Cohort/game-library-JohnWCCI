using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GameDataLibrary
{
    /// <summary>
    /// data Model for publisher
    /// </summary>
    [Table("Publishers")]
    public class PublisherModel :BaseModel
    {
        

        /// <summary>
        /// List of BoardGames published by this publisher
        /// </summary>
        public virtual IEnumerable<BoardGameModel>? BoardGames { get; set; }

        /// <summary>
        /// This class as json data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<PublisherModel>(this);
        }
    }
}
