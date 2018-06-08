//純數字
function checkPhone(invoiceNumber) {
    var regularExpression = /^[0-9]+$/;

    return invoiceNumber.match(regularExpression);
}
//字母+數字
function checkAccount(invoiceNumber) {
    var regularExpression = /^[a-zA-Z0-9]+$/;

    return invoiceNumber.match(regularExpression);
}
//mail格式
function checkmail(invoiceNumber) {
    var regularExpression = /^[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

    return invoiceNumber.match(regularExpression);
}
//身分證格式
function checkTwID(id) {
    //建立字母分數陣列(A~Z)  
    var city = new Array(
         1, 10, 19, 28, 37, 46, 55, 64, 39, 73, 82, 2, 11,
        20, 48, 29, 38, 47, 56, 65, 74, 83, 21, 3, 12, 30
    )
    id = id.toUpperCase();
    // 使用「正規表達式」檢驗格式  
    if (id.search(/^[A-Z](1|2)\d{8}$/i) == -1) {
        //alert('基本格式錯誤');  
        return false;
    } else {
        //將字串分割為陣列(IE必需這麼做才不會出錯)  
        id = id.split('');
        //計算總分  
        var total = city[id[0].charCodeAt(0) - 65];
        for (var i = 1; i <= 8; i++) {
            total += eval(id[i]) * (9 - i);
        }
        //補上檢查碼(最後一碼)  
        total += eval(id[9]);
        //檢查比對碼(餘數應為0);  
        return ((total % 10 == 0));
    }
}
//有效日期
function check_date(objDate, tmperrmsg) {

    var ischrome = (navigator.userAgent.toLowerCase().indexOf('chrome') != -1)
    var isfirefox = (navigator.userAgent.toLowerCase().indexOf('firefox') != -1)
    var chkDate = new Date(objDate.value.trim());
    //alert(objDate.value.trim() + 'xx');
    var tmpgetyear = chkDate.getYear();

    if (tmpgetyear.toString().length == 3) {
        tmpgetyear = chkDate.getYear() + 1900;
    }


    //西元年
    //if(ischrome==true || isfirefox==true)
    //{ tmpgetyear = chkDate.getYear()+1900;}

    if (tmpgetyear < 100) {
        if ((chkDate.getMonth() + 1) >= 10) {
            if (chkDate.getDate() >= 10) {
                testDat1 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = testDat1
                testDat3 = testDat1
                testDat4 = testDat1
            }
            else {
                testDat1 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/0' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat3 = testDat1
                testDat4 = testDat1
            }
        }
        else {
            if (chkDate.getDate() >= 10) {
                testDat1 = tmpgetyear + '/0' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat3 = testDat1
                testDat4 = testDat1
            }
            else {
                testDat1 = tmpgetyear + '/0' + (chkDate.getMonth() + 1) + '/0' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/0' + chkDate.getDate(); //組合日期，消去時間。
                testDat3 = tmpgetyear + '/0' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat4 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
            }
        }
    }
    else {
        if ((chkDate.getMonth() + 1) >= 10) {
            if (chkDate.getDate() >= 10) {
                testDat1 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = testDat1
                testDat3 = testDat1
                testDat4 = testDat1
            }
            else {
                testDat1 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/0' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat3 = testDat1
                testDat4 = testDat1
            }
        }
        else {
            if (chkDate.getDate() >= 10) {
                testDat1 = tmpgetyear + '/0' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat3 = testDat1
                testDat4 = testDat1
            }
            else {
                testDat1 = tmpgetyear + '/0' + (chkDate.getMonth() + 1) + '/0' + chkDate.getDate(); //組合日期，消去時間。
                testDat2 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/0' + chkDate.getDate(); //組合日期，消去時間。
                testDat3 = tmpgetyear + '/0' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
                testDat4 = tmpgetyear + '/' + (chkDate.getMonth() + 1) + '/' + chkDate.getDate(); //組合日期，消去時間。
            }
        }

    }

    testDat = chkDate.toString();

    if (testDat == 'NaN' && testDat.indexOf('/') < 0) {
        return tmperrmsg + "格式錯誤，請使用格式（yyyy/mm/dd）\n";
    }
    else if (testDat1 != objDate.value.trim() && testDat2 != objDate.value.trim() && testDat3 != objDate.value.trim() && testDat4 != objDate.value.trim()) {
        return tmperrmsg + "格式錯誤，請使用格式（yyyy/mm/dd）\n";
    }
    else {
        return "";
    }
}
//顯示欄位驗證錯誤訊息
function showErrorMessage(Target, msg) {
    $(Target).html(msg);
}