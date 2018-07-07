angular.module('asposeApp').directive('dropdownMultiselect', ['$window', '$http', function ($window, $http) {
    return {
        restrict: 'E',
        scope: {
            model: '=',
            options: '=',
            pre_selected: '=preSelected',
            dropdownTitle: '='
        },
        template: "<div data-ng-class='{open: open}'>" +
        "<button type='button' class='form-control' data-ng-click='open=!open;openDropDown()' oncontextmenu='return false;' style='position:inherit'>{{defaultText}}{{newTitle}}</button>" +
            "<button type='button' class='btn btn-small dropdown-toggle' data-ng-click='open=!open;openDropDown()' oncontextmenu='return false;' onkeypress='return false;' onpaste='return false;' style='float:right;margin-top:-43px;padding:10px'><span class='caret'></span></button>" +
            "<ul class='dropdown-menu scrollable-menu' aria-labelledby='dropdownMenu' style='position:inherit;width: 100%;padding-left: 10px;'>" +
            "<li><input type='checkbox' data-ng-change='checkAllClicked()' style='margin-right:5px' data-ng-model=checkAll> Select All</li>" +
            "<li class='divider'></li>" +
            "<li data-ng-repeat='option in options'> <input type='checkbox' data-ng-change='setSelectedItem(option.Location)' style='margin-right:10px' ng-model='selectedItems[option.Location]'>{{option.Location}}</li>" +
            "</ul>" +
            "</div>",
        link: function ($scope) {
            $scope.selectedItems = {};
            $scope.checkAll = false;

            init();

            function init() {
                $scope.defaultText = "Select Locations";
                $scope.newTitle = "";
                for (var i = 0; i < $scope.pre_selected.length; i++) {
                    $scope.model.push($scope.pre_selected[i].id);
                    $scope.selectedItems[$scope.pre_selected[i].id] = true;
                }
                if ($scope.pre_selected.length == $scope.options.length) {
                    $scope.checkAll = true;
                }
                angular.forEach($scope.model, function (val, index) {
                    if ($scope.newTitle != "")
                        $scope.defaultText = "";
                    $scope.newTitle += ", " + val;
                    $scope.newTitle = $scope.newTitle.substring(1, $scope.newTitle.length);
                })
            }

            $scope.openDropDown = function () {
                return false;
            }
            $scope.closeDropDown = function () {
                return false;
            }

            $scope.checkAllClicked = function () {
                if ($scope.checkAll) {
                    selectAll();
                } else {
                    deselectAll();
                }
                if ($scope.newTitle != "")
                    $scope.defaultText = "";
                else
                    $scope.defaultText = "Select Locations";
            }

            function selectAll() {
                $scope.newTitle = "All Selected";
                $scope.model = [];
                $scope.selectedItems = {};
                angular.forEach($scope.options, function (option) {
                    $scope.model.push(option.Location);
                });
                angular.forEach($scope.model, function (Location) {
                    $scope.selectedItems[Location] = true;
                });

            };

            function deselectAll() {
                $scope.newTitle = "None Selected";
                $scope.model = [];
                $scope.selectedItems = {};
            };

            $scope.setSelectedItem = function (id) {
                var filteredArray = [];
                $scope.newTitle = "";
                if ($scope.selectedItems[id] == true) {
                    $scope.model.push(id);
                } else {
                    filteredArray = $scope.model.filter(function (value) {
                        return value != id;
                    });
                    $scope.model = filteredArray;
                    $scope.checkAll = false;
                }
                angular.forEach($scope.model, function (val, index) {
                    $scope.newTitle += ", " + val;
                    if ($scope.newTitle.substring(0, 1) == ",")
                        $scope.newTitle = $scope.newTitle.substring(2, $scope.newTitle.length);

                    //else
                    //    $scope.newTitle=
                })
                if ($scope.newTitle != "")
                    $scope.defaultText = "";
                else
                    $scope.defaultText = "Select Locations";
                return false;
            };

        }
    }
}]);