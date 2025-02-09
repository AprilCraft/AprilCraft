let g_header_height
let g_list_height

let loading_frame_mini = `<div class="loading-frame-mini absolute flex flex-col justify-center items-center text-center">
                    <div class="loadframe flex flex-col items-center ">
                        <svg id="spinner" class=" h-5 w-5 animate-spin text-white" xmlns="http://www.w3.org/2000/svg" fill="none"
                            viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                    </div>
                </div>`


let thumbnail = (id, thumb) => {
    return `<a class="thumbnail h-[85px] min-w-[100px] bg-white" id="${id}">
                <img src="${thumb.thumbInBase64}" alt="" class="thumb w-full h-full">
            </a>`
}

var loadingframe = $('.loading-frame')
var loadframemini = $('.loading-frame-mini')

function getBase64(src, id, callback) {
    var image = new Image()
    image.crossOrigin = 'Anonymous'
    image.onload = function () {
        var canvas = document.createElement('canvas')
        var context = canvas.getContext('2d')
        canvas.height = this.naturalHeight
        canvas.width = this.naturalWidth
        context.drawImage(this, 0, 0)
        var dataURL = canvas.toDataURL('image/jpeg')
        callback(id, dataURL)
    }
    image.src = src
}

let slideImages = []

function DownloadSlideImages() {
    let i = 0
    while (i < 12) {
        getBase64(location.origin + "/assets/images/slides/" + i + ".jpg", i, (id, dataURL) => {
            slideImages.push({id: id, image: dataURL})
        })
        i++
    }
}

DownloadSlideImages()

/*let getThumbs = () => {
    $.ajax({
        type: "post",
        url: `${BaseURL}images/getImgThumbs?gallery=${gallery}`,
        error: (error) => {
            console.log(error)
        },
        success: function (response) {
            console.log(response)
            let thumb_list = $('.thumb-list')
            thumb_list.html('')
            let i = 0
            Thumbs = response
            response.forEach(item => {
                thumb_list.append(thumbnail(i, item));
                i++
            });
        }
    }).done(() => {
        $('.thumbnail').click((e) => {
            $('.thumbnail').removeClass('selected')
            let id = e.target.id || e.target.parentElement.id
            $('#' + id).addClass('selected')
            $('#' + id).append(loading_frame_mini);

            $.ajax({
                type: 'get',
                url: `https://localhost:7201/assets/images/thumbnails/${id}`,
                xhrFields: {
                    responseType: 'blob'
                },
                error: (error) => {
                    console.log(error)
                },
                success: function (response) {
                    const url = window.URL || window.webkitURL
                    const src = url.createObjectURL(response)
                    var view = $('.view-box')
                    view.html(`<img src="${src}" alt="" class="img-main max-h-[${calc_height(g_header_height, g_list_height)}px]">`)
                }
            })
        })
        loadingframe.addClass('hidden')
    });
}
*/
function calc_height(h, l) {
    return (window.innerHeight - (h + l))
}

$(document).ready((e) => {
	//setTimeout(displayCrafts, 5000)

	
})

function displayCrafts() {
	let crafts = $('.craft')
	fadeInCraft(crafts, 0, crafts.length)
}

function fadeInCraft(crafts, index, length) {
	if (index > length) return
	$(crafts[index]).fadeIn(500, (e) => {
		//$(crafts[index]).css('display', 'flex')
		fadeInCraft(crafts, index + 1, length)
	})
}

function getSecs() {
    return Date.now()
}

function init(data) {
    let ac_track_key = "ac_track_key", trackIdey = 'ac_trackid_key'
    let trackId = parseInt(localStorage.getItem(trackIdey)) || getSecs()
    if (localStorage.hasOwnProperty(ac_track_key) != true) {
        navigator.geolocation.getCurrentPosition((location) => {
            
        })
        let visitData = JSON.stringify({ trackId: trackId, hasVisited: true, location: { platform: navigator.platform, userAgent: navigator.userAgent, appVersion: navigator.appVersion, roductSub: navigator.productSub, vendor: navigator.vendor } })
        localStorage.setItem(trackIdey, trackId)
        localStorage.setItem(ac_track_key, visitData)
        DotNet.invokeMethodAsync('craft-ui', 'SaveVisitor', visitData, trackId, location.origin).then(result => {
            console.log(result)
        })
    }
    g_header_height = parseInt(getComputedStyle(document.querySelector('.g-header')).height.replace('px', ''))
    g_list_height = parseInt(getComputedStyle(document.querySelector('.thumb-list')).height.replace('px', ''))
    $('.g-main').css('max-height', `${window.innerHeight}px`)

    $('img').on('dragstart', function (event) { event.preventDefault(); });
    /*$('.thumb-list').mousewheel(function (e, delta) {
        this.scrollLeft -= (delta * 30);
        e.preventDefault();
    });*/

    $('.thumbnail').click((e) => {
        $('.thumbnail').removeClass('selected')
        let id = e.target.closest('.thumbnail').id.substring(6)
        //console.log(id, data)
        $('#thumb_' + id).addClass('selected')
        $('#thumb_' + id).append(loading_frame_mini);

        //data = new Array(data.split(','))// JSON.parse(data)
        //console.log(id)
        getImage(id, data)
    })
    $('.craft').click((e) => {
        $('.craft').removeClass('selected')
        let id = e.target.closest('.craft').id.substring(6)
        //console.log(id, data)
        $('#craft_' + id).addClass('selected')

        //data = new Array(data.split(','))// JSON.parse(data)
        //console.log(id)
        $('.g-main').fadeIn(100, (e) => {
            $('.g-main').css('display', 'grid')
            getImage(id, data)
        })
    })

    $('.exit').click((e) => {
        $('.g-main').fadeOut(100, (e) => {
            $('.g-main').css('display', 'none')
            //getImage(id, data)
        })
    })

    contents = $('div.contents');

    $('.next').click((e) => {
        bgFadeOut()
        currentId++
        nextContent(e)
    })

    $('.previous').click((e) => {
        bgFadeOut()
        currentId--
        previousContent(e)
    })


    $('.dot').click((e) => {
        let id = e.target.id / 1;
        bgFadeOut()
        clearInterval(animWaitTime)
        animWaitTime = setWaitInterval()
        currentId = id
        changeContent()
    })
}

function getImage(id, list) {
    let thumbId = '#thumb_' + id;
    $.ajax({
        type: 'get',
        url: `${location.origin}/assets/images/designs/${list[id]}`,
        xhrFields: {
            responseType: 'blob'
        },
        error: (error) => {
            console.log(error)
        },
        success: function (response) {
            const url = window.URL || window.webkitURL
            const src = url.createObjectURL(response)
            let view = $('.view-box')
            view.html(`<img src="${src}" alt="" class="img-main max-h-[${calc_height(g_header_height, g_list_height)}px] ">`).ready((e) => {
                let thumbList = document.querySelector('.thumb-list')
                let selected = document.querySelector(thumbId)
                selected.classList.add('selected')
                thumbList.scrollLeft = selected.offsetLeft - (innerWidth / 2) + (parseInt(getComputedStyle(selected).width.replace('px', '')) / 2)
            })
        }
    }).done(() => {
        $(thumbId + ' .loading-frame-mini').remove()
    })
}


let slideMessages = [
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Turning your creative visions into reality</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Capturing the essence of each season for you</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Delivering top-notch craftsmanship every time</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Making your brand shine brighter than the rest</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Turning your ideas into stunning visuals</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Transforming your visions into reality</p></div>',

    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Bringing your concepts to life through prototypes</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Crafting memories that last a lifetime</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Crafting excellence for you</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Making your ideas come alive</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Your vision, our creation</p></div>',
    '<div class="content"><h1 class="craft-name text-5xl sm:text-8xl drop-shadow-md ksh">AprilCraft</h1><p class="text-lg my-3 sm:my-4 nr font-[500]">Innovating for a brighter future</p></div>'
]

let currentId = 0

let contents = $('div.contents');
let changeContent = (t = 'n') => {
    if (slideImages.length < 1) return;

    firstTime = false;
    let m = innerWidth/2
    if (t == 'p') m = -1*m
    if (contents.children()[0] == null) {
        contents.html(slideMessages[currentId]).ready(() => {
            $('.dot').animate({
                backgroundColor: '#bbb'
            },
                {
                    duration: 200,
                    easing: 'linear',
                    complete: () => {
                        $('.dot').removeClass('active').ready(() => {
                            $(`#0`).addClass('active')
                        })
                    }
                }
            )
            $(contents.children()[0]).animate({
                opacity: '1',
            },
                {
                    duration: 500,
                    easing: 'linear',
                    complete: () => {
                        //currentId++
                    }
                })
        })
        return
    }

    //console.log(contents.children()[0])
    $(contents.children()[0]).animate({
        opacity: '0',
        marginInlineStart: `${m}px`
    },
        {
            duration: 500,
            easing: 'linear',
            complete: () => {
                if (currentId > 11) currentId = 0
                contents.html(slideMessages[currentId]).ready(() => {
                    $('.dot').animate({
                        backgroundColor: '#bbb'
                    },
                        {
                            duration: 100,
                            easing: 'linear',
                            complete: () => {
                                $('.dot').removeClass('active').ready(() => {
                                    $(`#${currentId}`).addClass('active')
                                })
                                    changeStarted = false;
                            }
                        }
                    )
                    $('div.main-bg, .main-bg-bg').css({
                        // 'transition': 'background-image 1.2s ease',
                        'background-image': `url(${slideImages.find(img => img.id == currentId).image})`,
                        // 'transition-timing-function': 'ease',
                        '-webkit-animation': 'fadein 1s',
                        '-moz-animation': 'fadein 1s',
                        '-ms-animation': 'fadein 1s', /* Internet Explorer */
                        '-o-animation': 'fadein 1s',/* Opera < 12.1 */
                        'animation': 'fadein 1s',
                        'animation': 'scaleOut 7.5s linear'
                    })
                    $(contents.children()[0]).animate({
                        opacity: '1',
                    },
                        {
                            duration: 500,
                            easing: 'linear',
                            complete: () => {

                            }
                        })
                })
            }
        }
    )
}

/*setInterval(() => {
    bgFadeOut()
    currentId++
    nextContent(PointerEvent)
}, 8000)*/

let firstTime = true, changeStarted = false

let animWaitTime = setWaitInterval()

function setWaitInterval() {
    return setInterval(() => {
        if (changeStarted) return
        changeStarted = true;
        bgFadeOut()
        currentId++
        changeContent()
        //nextContent(PointerEvent)
    }, 10000)
}

let nextContent = (e) => {
    changeContent()
}

let previousContent = (e) => {
    if (currentId < 0) {
        currentId = 11
    }
    changeContent('p')
}

function bgFadeIn() {
    $('div.main-bg, .main-bg-bg').css({
        // 'transition': 'background-image 1.2s ease',
        'background-image': `url(${slideImages.find(img => img.id == currentId).image})`,
        // 'transition-timing-function': 'ease',
        '-webkit-animation': 'fadein 1s',
        '-moz-animation': 'fadein 1s',
        '-ms-animation': 'fadein 1s', /* Internet Explorer */
        '-o-animation': 'fadein 1s',/* Opera < 12.1 */
        'animation': 'fadein 1s',
        'animation': 'scaleOut 30s'
    })
}

function bgFadeOut() {
    $('div.main-bg, .main-bg-bg').css({
        // 'transition': 'background-image 1.2s ease',
        // 'transition-timing-function': 'ease',
        '-webkit-animation': 'fadeout 1s',
        '-moz-animation': 'fadeout 1s',
        '-ms-animation': 'fadeout 1s', /* Internet Explorer */
        '-o-animation': 'fadeout 1s',/* Opera < 12.1 */
        'animation': 'fadeout 1s'
        //'transform': 'scale(1.01)'
    })
}
