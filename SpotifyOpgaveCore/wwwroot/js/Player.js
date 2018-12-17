function Play(music, id) {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '/Player/Play',
        });
}
function Stop(music, id) {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '/Player/Pause',
        });
}
function VolumeUp() {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '/Player/VolumeUp',

        });
}
function VolumeDown() {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '/Player/VolumeControl',
        });
}
function Next() {

    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '/Player/SkipToNext',
        });
}
function Previous() {
    $.ajax(
        {
            type: "POST", //HTTP POST Method
            url: '/Player/SkipToPrev',
        });
}
function Forward1(music, id) {
    var audio = $("#" + id);
    audio.prop("currentTime", audio.prop("currentTime") + 1);
}
function Backward1(music, id) {
    var audio = $("#" + id);
    audio.prop("currentTime", audio.prop("currentTime") - 1);
}