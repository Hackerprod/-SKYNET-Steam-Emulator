using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Pages
{
    public class jquery
    {
        public static string js()
        {
            string result = @"

/* CSS Helper Classes */

/* Margin */
.m-0 {
    margin: 0;
}

.mr-4 {
    margin-right: 4px;
}

.mr-8 {
    margin-right: 8px;
}

.mr-24 {
    margin-right: 24px;
}

.mt-16 {
    margin-top: 16px;
}


/* Padding */
.p-0 {
    padding: 0;
}

.pt-4 {
    padding-top: 4px;
}

.pt-8 {
    padding-top: 8px;
}

.px-24 {
    padding-left: 24px;
    padding-right: 24px;
}

/* Other */
.cursor-pointer {
    cursor: pointer;
}

/* Colors */
.fg-light-gray {
    color: #dddddd;
}

.fg-mid-gray {
    color: #9b9b9b;
}

";
            return result;
        }
    }
}
