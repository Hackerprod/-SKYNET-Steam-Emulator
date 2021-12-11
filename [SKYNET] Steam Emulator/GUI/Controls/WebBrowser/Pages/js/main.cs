using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Pages
{
    public class main
    {
        public static string js()
        {
            string result = @"

// Y axis scroll speed
var velocity = 0.3;

function update(){ 
    var pos = $(window).scrollTop(); 
    $('body').each(function() { 
        var $element = $(this);
        // subtract some from the height b/c of the padding
        $(this).css('backgroundPosition', '0 ' + Math.round((0 - pos) * velocity) + 'px'); 
    }); 
};

$(window).bind('scroll', update);
";
            return result;
        }
    }
}
