/**
Javascript Util Collection
 */


Function.prototype.method = function (name, func) {
    this.prototype[name] = func;
    return this;
};

//动态添加JS文件
function append_js(url,js_file_id)
{
        var obj = $(js_file_id);
        if(obj){
                obj.parentNode.removeChild(obj);
		}
        var newscript = document.createElement("script");
        newscript.type = "text/javascript";
        newscript.src = url;
        newscript.id = js_file_id;
        document.body.appendChild(newscript);
}

//$符获得对象
function $(objname){
	return document.getElementById(objname);
}

//获得对象X坐标
function objPosX(obj){
	var left = 0;
	if (obj.offsetParent) {
		while (obj.offsetParent) {
			left += obj.offsetLeft;
			obj = obj.offsetParent;
		}
	} else if (obj.x) eft += obj.x;
	return left;
}

//获得对象Y坐标
function objPosY(obj){
	var top = 0;
	if (obj.offsetParent) {
		while (obj.offsetParent) {
			top += obj.offsetTop;
			obj = obj.offsetParent;
		}
	} else if (obj.y) top += obj.y;
	return top;
}