using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WordSearch.Models
{
    public class DebugLog
    {
        [Key]
        public int Id { get; set; }
        // event created time
        public DateTime CreatedDate { get; set; }
        // log text
        public string Log { get; set; }

        public DebugLog()
        {
            Id = 0;
            CreatedDate = DateTime.Now;
        }
    }
}
