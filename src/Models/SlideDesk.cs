using System.Collections.Generic;

namespace FP.Statiq.RevealJS.Models
{
    public class SlideDesk
    {
        public IList<Section> Sections { get; set; }

        public string Access { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public string Theme { get; set; }

        public string Password { get; set; }

        public string Copyright { get; set; }

        public Multiplex Multiplex { get; set; }
    }
}
