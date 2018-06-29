window.onload = function () {
    document.getElementById("deletePost").onclick = function () {
        var rtn = confirm("Are you sure you wish to delete this post?")
        return rtn;
    }
}
