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