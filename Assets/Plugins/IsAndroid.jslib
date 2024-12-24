mergeInto(LibraryManager.library, {
    IsAndroid: function () {
      var ua = window.navigator.userAgent.toLowerCase();
      if(ua.indexOf("android") !== -1){
        return 1;  
      }else{
        return 0;
      }
    }
});