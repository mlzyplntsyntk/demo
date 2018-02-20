using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    /**
     * Base Entity Class for Models.
     * It has an Identity and a Creation Time field
     * It makes it easier to deal with Generics when you have
     * an abstract class for all of your models.
     */
    public class AbstractEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime CreationTime { get; set; }

        public AbstractEntity()
        {
            this.Id = 0;
            this.CreationTime = DateTime.Now;
        }
    }
}
