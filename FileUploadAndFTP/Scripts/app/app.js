var asposeApp = angular.module('asposeApp', ['ngRoute', 'angularUtils.directives.dirPagination', 'ui.bootstrap']);

asposeApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
      when('/login', {
          templateUrl: 'login.html',
          controller: 'loginController'
      }).
      when('/file-list', {
          templateUrl: 'filesList.html',
          controller: 'filesListController'
      }).
    when('/file-upload', {
        templateUrl: 'fileUpload.html',
        controller: 'fileUploadController'
          }).
          when('/manage-location', {
              templateUrl: 'Locations.html',
              controller: 'statesController'
          }).
    when('/manage-states', {
        templateUrl: 'manageLocations.html',
        controller: 'statesController'
    }).
    otherwise({
        redirectTo: '/login'
    });
  }]);
asposeApp.directive('checkFileSize', function () {
    return {
        link: function (scope, elem, attr, ctrl) {
            function bindEvent(element, type, handler) {
                if (element.addEventListener) {
                    element.addEventListener(type, handler, false);
                } else {
                    element.attachEvent('on' + type, handler);
                }
            }

            bindEvent(elem[0], 'change', function () {
                if (this.files[0].size > 1024) {

                    alert('File size should not be greated then 1MB');
                }
            });
        }
    }
});


asposeApp.controller('loginController', ['$scope', '$http', '$location', '$window', '$rootScope', function ($scope, $http, $location, $window, $rootScope) {
    $scope.customers = [];
    // GetAllCustomers();
    function GetAllCustomers() {
        fnshowSearch_Progress();
        $http.get('/api/upload/getallcustomers').success(function (data) {
            console.log("success!");
            console.log(data);
            $scope.customers = data;
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    };
    var countstatus = false;

    //Latest
    $scope.loginUser = function () {
        $scope.loginError = '';
        fnshowSearch_Progress();
        if ($scope.username == '' || $scope.password == '' || $scope.username == undefined || $scope.password == undefined) {
            fnhideSearch_Progress();
            console.log('No Credentials');
            $scope.loginError = 'Please fill User Name and Password';
        }
        else {
            $http.post('/api/upload/login', { username: $scope.username, password: $scope.password }).then(function (data) {
                if (data.statusText == 'OK') {
                    console.log('customer login successful');
                    localStorage.setItem('currentUserId', data.data.UserId);
                    localStorage.setItem('role', data.data.Role);
                    localStorage.setItem('customerId', data.data.CustomerId);
                    localStorage.setItem('currentUser', data.data.UserName);
                    $scope.CustomerName = data.data.CustomerName;
                    fnhideSearch_Progress();
                    $window.location.href = "#/file-list";
                }
                else if (data.statusText == 'Unauthorized') {
                    fnhideSearch_Progress();
                    console.log('Login Failed,wrong Credentials');
                    $scope.loginError = 'Login Failed,wrong Credentials';
                }
                else {
                    fnhideSearch_Progress();
                    alert("an error occurred");
                }
            }, function (data) {
                fnhideSearch_Progress();
                if (data.statusText == 'Unauthorized') {
                    console.log('Login Failed,wrong Crendentials');
                    $scope.loginError = 'Login Failed,wrong Credentials';
                }
                else
                    alert("an error occured");
            });
        }
    };

    $scope.cancel = function () {
    };
}]);

asposeApp.controller('filesListController', ['$scope', '$http', '$rootScope', function ($scope, $http, $rootScope) {
    console.log("filesListController");
    $scope.currentPage = 1;
    $scope.itemsPerPage = 10;
    $scope.AdminRole = 1;
    $scope.currentUser = localStorage.getItem('currentUser');
    $scope.currentUserId = localStorage.getItem('currentUserId');
    $scope.Role = localStorage.getItem('role');
    $scope.CustomerId = localStorage.getItem('customerId');

    $scope.SetPage = function (index) {
        if ($scope.currentPage == 1) {
            return index + 1;
        }
        else {
            var val = ($scope.currentPage - 1) * $scope.itemsPerPage + index;
            return val + 1;
        }
    }

    $scope.cancel = function () {
        alert("cancel");
    };


    $scope.uploadFile = function () {
        var file = $scope.myFile;
        console.log('file is ');
        console.dir(file);

        var uploadedFiles = [];
        //localStorage["uploadedFiles"] = file;
        //localStorage.setItem('uploadedFiles', file);

        var uploadUrl = "/fileUpload";
        var dataObj = {
            fileName: file.name,
            fileStatus: "reviewing",
            uploadedDate: "05.02.2015",
            customerId: $scope.customer.CustomerId
        };
        $scope.filesList = {
            id: "",
            customerAdminId: "",
            fileName: "",
            fileStatus: "",
            uploadedDate: "",
            fileSize: "",
            locationName: "",
            filePath: "",
            locationIds: "",
            customerId: "",

        }
        $scope.filesList.push(JSON.stringify(dataObj));
        //fileUpload.uploadFileToUrl(file, uploadUrl);
    };


    $scope.displayFilesList = function () {
        fnshowSearch_Progress();
        $http.post('/api/upload/GetAllFileList', { CustomerId: $scope.CustomerId, Role: $scope.Role }).success(function (data) {
            console.log("success!");
            console.log(data);
            $scope.filesList = data;
            //if ($scope.currentUserId == "-1")
            //    $scope.filesList = data;
            //else {
            //    $scope.filesListByID = data;
            //    if ($scope.filesList == undefined)
            //        $scope.filesList = [];
            //    for (var i = 0; i < $scope.filesListByID.length; i++) {
            //        if ($scope.currentUser == $scope.filesListByID[i].customerName) {
            //            $scope.filesList.push($scope.filesListByID[i]);
            //        }
            //    }
            //}

            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    }


    //$scope.displayFilesList = function () {
    //    fnshowSearch_Progress();
    //    $http.get('/api/upload/GetFileList').success(function (data) {
    //        console.log("success!");
    //        console.log(data);
    //        if ($scope.currentUserId=="-1")
    //            $scope.filesList = data;
    //        else {
    //            $scope.filesListByID = data;
    //            if ($scope.filesLikst == undefined)
    //                $scope.filesList = [];
    //            for (var i = 0; i < $scope.filesListByID.length; i++)
    //            {
    //                if ($scope.currentUser == $scope.filesListByID[i].customerName)
    //                {
    //                    $scope.filesList.push($scope.filesListByID[i]);
    //                }
    //            }
    //        }

    //        fnhideSearch_Progress();
    //    }, function (data) {
    //        fnhideSearch_Progress();
    //        alert("an error occured");
    //    });
    //}



    $scope.approveFile = function (file) {
        fnshowSearch_Progress();
        //$http.post('/api/upload/approvefile', file).success(function (data) {
        $http.post('/api/upload/ApproveFile', file).success(function (data) {
            console.log("approval success for file: " + file.id);
            $scope.displayFilesList();
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    }

    $scope.deleteFile = function (file) {
        if (confirm('Are you sure? want to delete!')) {
            fnshowSearch_Progress();
            $http.post('/api/upload/DeleteFile', file).success(function (data) {
                console.log("deletion success for file: " + file.id);
                $scope.displayFilesList();
                fnhideSearch_Progress();
            }, function (data) {
                fnhideSearch_Progress();
                alert("an error occured");
            });
        }
    }

    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    }

    $scope.displayFilesList();

    paypal.Button.render({

        env: 'production', // sandbox | production

        // PayPal Client IDs - replace with your own
        // Create a PayPal app: https://developer.paypal.com/developer/applications/create
        client: {
            sandbox: 'AYeNfom2iXK0yJ5sIKNF_Wsd396FCj5U4fmQ9FPIoNhf4wZmvynxptpibuJHJhtblEDJGWjAU4SA84Ow',
            production: 'AdjDfDlV2ba6_MnHxPLOhQGGuf7S1LD7QGLytgYXe64gj3k6iZHPQp_HefRsMn4SnWfkORz8tE2gkjwN'
        },

        // Show the buyer a 'Pay Now' button in the checkout flow
        commit: true,

        // payment() is called when the button is clicked
        payment: function (data, actions) {

            // Make a call to the REST api to create the payment
            return actions.payment.create({
                payment: {
                    transactions: [
                        {
                            amount: { total: $scope.feeAmount, currency: 'USD' }
                        }
                    ]
                }
            });
        },

        // onAuthorize() is called when the buyer approves the payment
        onAuthorize: function (data, actions) {

            // Make a call to the REST api to execute the payment
            return actions.payment.execute().then(function () {
                window.alert('Payment Complete!');
            });
        }

    }, '#paypal-button-container');
}]);

asposeApp.controller('fileUploadController', ['$scope', '$http', '$rootScope', '$window', function ($scope, $http, $rootScope, $window) {
    console.log("fileUploadController");
    $scope.AdminRole = 1;
    $scope.CustomerId = localStorage.getItem('customerId');
    $scope.Role = localStorage.getItem('role');

    //$scope.addFile = function () {
    //    fnshowSearch_Progress();
    //    $http.post("api/upload/uploadfile").success(function (data) {
    //        console.log("success!");
    //        console.log(data);
    //        alert(data);
    //    });
    //}
    $scope.AddFile = function () {
        var formData = new FormData($("#fUpload")[0]);
        $.ajax({
            url: '/api/upload/insertFile',
            type: 'post',
            // Form data
            data: formData,
            //Options to tell JQuery not to process data or worry about content-type
            cache: false,
            contentType: false,
            processData: false
        }).success(function (d) {
            $window.location.href = "#/file-list";
        });

    };

    $scope.addNewFile = function () {
        var formData = new FormData($("#fUpload")[0]);
        if ($scope.Role != $scope.AdminRole)
            $scope.customer = $scope.CustomerId;
        if ($scope.selected_items.length > 0) {
            var newArray = $scope.selected_items.filter(function (v) { return v !== '' });
            $scope.selected_items = newArray;
        }
        if ($scope.selected_items.length > 0 && $scope.customer != null && $scope.fileName != null) {
            $.ajax({
                url: '/api/upload/uploadfile',
                type: 'post',
                // Form data
                data: formData,
                //Options to tell JQuery not to process data or worry about content-type
                cache: false,
                contentType: false,
                processData: false
            }).success(function (d) {
                if (d == 'Success')
                    $window.location.href = "#/file-list";
                else {
                    var isProceed = confirm(d);
                    if (isProceed)
                        $scope.AddFile();


                }
            });
        }
        else
            alert("Please fill all the fields");
    };


    //date picker
    $scope.clear = function () {
        $scope.News.NewsDate = null;
    };

    $scope.toggleMin = function () {
        $scope.maxDate = $scope.maxDate ? null : new Date();
    };

    $scope.toggleMin();

    $scope.open = function ($event) {
        $scope.status.opened = true;
    };

    $scope.status = {
        opened: false
    };

    $scope.states = [];

    $scope.GetAllStates = function () {
        fnshowSearch_Progress();
        $http.get('/api/upload/getallstates').success(function (data) {
            console.log("success!");
            console.log(data);
            $scope.states = data;
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    };
    $scope.member = {
        states: [{
            id: ""
        }]
    };


    $scope.customer = null;
    $scope.customers = [];
    $scope.GetAllCustomers = function () {
        fnshowSearch_Progress();
        debugger;
        $http.get('/api/upload/getallcustomers').success(function (data) {
            console.log("success!");
            console.log(data);
            $scope.customers = data;
            //if ($scope.currentUserId == "-1")
            //{
            //    $scope.customers = data;
            //}
            //else {
            //    $scope.allcust = data;
            //    if ($scope.customers == undefined)
            //        $scope.customers = [];
            //    for (var i = 0; i < $scope.allcust.length; i++) {
            //        if ($scope.currentUser == $scope.allcust[i].CustomerName) {
            //            $scope.customers.push($scope.allcust[i]);
            //        }
            //    }
            //}
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    };



    $scope.selected_items = [];
    $scope.displayItem = JSON.stringify($scope.selected_items);
    //$scope.displaySettings = { displayProp: 'label', idProp: 'label' };
    $scope.currentUser = localStorage.getItem('currentUser');
    $scope.currentUserId = localStorage.getItem('currentUserId');

    $scope.GetAllStates();
    $scope.GetAllCustomers();
    $scope.fileName = null;
    $scope.ValidateFileName = function (oInput) {
        var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png", ".mp4", ".tiff"];
        var regex = new RegExp("^[a-zA-Z0-9]+$");
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                var msg = "";
                var filename = oInput.value.split('\\').pop();
                var fname = filename.split('.')[0];
                if (fname.length > 15) {
                    blnValid = false;
                    msg = "Sorry, " + filename + " is invalid. File name exceeds 15 characters."
                }
                else if (!regex.test(fname)) {
                    blnValid = false;
                    msg = "Sorry, " + filename + " is invalid. File name contains special characters/space(s)"
                }
                else {
                    msg = "Sorry, " + filename + " is invalid, allowed extensions are: " + _validFileExtensions.join(", ")
                    for (var j = 0; j < _validFileExtensions.length; j++) {
                        var sCurExtension = _validFileExtensions[j];
                        if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                            blnValid = true;
                            break;
                        }
                    }
                }

                if (!blnValid) {
                    alert(msg);
                    oInput.value = "";
                    return false;
                }
            }
        }
        $scope.fileName = filename;
        return true;
    };

}]);

asposeApp.controller('statesController', ['$scope', '$http', function ($scope, $http) {
    console.log("statesController");
    debugger;
    $scope.currentUser = localStorage.getItem('currentUser');
    $scope.currentUserId = localStorage.getItem('currentUserId');

    $scope.statepattern = "";
    $scope.stateslist = [];
    $scope.saveText = "Save";
    $scope.StateInfo = {
        id: "0",
        Location: "",
        Street: "",
        City: "",
        State: "",
        Zip: "",
        PvtCustName: "",
        LocationVendor: "",
        Country: "",
        Trafic: "",
        Customer: "",
        
    };

    $scope.GetAllCustomers = function () {
        fnshowSearch_Progress();
        debugger;
        $http.get('/api/upload/getallcustomers').success(function (data) {
            console.log("success!");
            console.log(data);
            $scope.customers = data;
            //if ($scope.currentUserId == "-1")
            //{
            //    $scope.customers = data;
            //}
            //else {
            //    $scope.allcust = data;
            //    if ($scope.customers == undefined)
            //        $scope.customers = [];
            //    for (var i = 0; i < $scope.allcust.length; i++) {
            //        if ($scope.currentUser == $scope.allcust[i].CustomerName) {
            //            $scope.customers.push($scope.allcust[i]);
            //        }
            //    }
            //}
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    };
    $scope.GetAllStates = function () {
        fnshowSearch_Progress();
        //$http.get('/api/upload/getallstates').success(function (data) {
        $http.get('/api/upload/GetAllLocations').success(function (data) {
            console.log("success!");
            console.log(data);
            $scope.stateslist = data;
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    };

    $scope.cleartext = function () {
        $scope.StateInfo.id = "";
        $scope.StateInfo.Location = "";
        $scope.StateInfo.CompanyName = "";
        // $scope.StateInfo.Address = "";
        $scope.StateInfo.Street = "";
        $scope.StateInfo.City = "";
        $scope.StateInfo.State = "";
        $scope.StateInfo.Zip = "";
        $scope.StateInfo.Live = "";
        $scope.StateInfo.LiveLoc = "";
        $scope.saveText = "Save";
    }

    $scope.SaveUpdateState = function () {
        fnshowSearch_Progress();
        if ($scope.StateInfo.Live == true)
            $scope.StateInfo.LiveLoc = 1;
        else
            $scope.StateInfo.LiveLoc = 0;

        //$http.post('/api/upload/saveupdatestate', $scope.StateInfo).then(function (data) {
        if ($scope.StateInfo.Location != '') {
            $http.post('/api/upload/SaveOrUpdateLocation', $scope.StateInfo).then(function (data) {
                console.log("save state!");
                console.log(data);
                $scope.cleartext();
                $scope.GetAllStates();
                fnhideSearch_Progress();
            }, function (data) {
                fnhideSearch_Progress();
                alert("an error occured");
            });
        }
        else {
            fnhideSearch_Progress();
            alert('Location is missing!');
        }
    }

    $scope.editState = function (state) {
        $scope.StateInfo.id = state.id;
        $scope.StateInfo.Location = state.Location;
        $scope.StateInfo.CompanyName = state.CompanyName;
        //$scope.StateInfo.Address = state.Address;
        $scope.StateInfo.Street = state.Street;
        $scope.StateInfo.City = state.City;
        $scope.StateInfo.State = state.State;
        $scope.StateInfo.Zip = state.Zip;
        $scope.StateInfo.Live = state.Live;
        $scope.StateInfo.LiveLoc = state.Live == true ? 1 : 0;
        $scope.saveText = "Update";
    }

    $scope.deleteState = function (state) {
        fnshowSearch_Progress();
        //$http.post('/api/upload/deletestate', state).then(function (data) {
        $http.post('/api/upload/DeleteLocation', state).then(function (data) {
            console.log("delete state!");
            console.log(data);
            $scope.GetAllStates();
            fnhideSearch_Progress();
        }, function (data) {
            fnhideSearch_Progress();
            alert("an error occured");
        });
    }

    $scope.sort = function (keyname) {
        $scope.sortKey = keyname;   //set the sortKey to the param passed
        $scope.reverse = !$scope.reverse; //if true make it false and vice versa
    }
    $scope.GetAllStates();
    debugger;
    alert("Get All Customer");
    $scope.GetAllCustomers();

    $scope.checkLive = function (liveStatus) {
        if (liveStatus == true)
            $scope.StateInfo.LiveLoc = 1;
        else
            $scope.StateInfo.LiveLoc = 0;
    }
    $scope.StateInfo.Customer = ddlCustomer.CustomerName;
}]);