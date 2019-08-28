using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Selectcon.Models
{
        public class DateFormatConverter : IsoDateTimeConverter
        {
            public DateFormatConverter(string format)
            {
                DateTimeFormat = format;
            }
        }

}