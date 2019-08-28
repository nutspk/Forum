using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Selectcon.Models
{
    public class ImageGalley
    {
        [JsonProperty(PropertyName = "QuestionID")]
        public int QuestionID { get; set; }

        [JsonProperty(PropertyName = "GalleyID")]
        public int GalleyID { get; set; }

        [JsonProperty(PropertyName = "ImgList")]
        public List<Image> ImgList { get; set; }
    }

    public class Image {

        [JsonProperty(PropertyName = "QuestionID")]
        public int QuestionID { get; set; } = 0;

        [JsonProperty(PropertyName = "AnswerID")]
        public int AnswerID { get; set; } = 0;

        [JsonProperty(PropertyName = "ImgID")]
        public int ImgID { get; set; }

        [JsonProperty(PropertyName = "ImgSrc")]
        public string ImgSrc { get; set; }
    }
}