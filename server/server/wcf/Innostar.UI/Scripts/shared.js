var shared = {
	bodyHeight: 0,
	refreshInterval: null,
	countTrying: 0,
	validatableForms: ["collectionItemForm", "createUserForm", "collectionCreationForm"],

	setFooter: function () {
		if ($(window).scrollTop() == 0) {
			$(window).scrollTop(1);
			var scrollTop = $(window).scrollTop();
			if (scrollTop > 0) {
				$('footer').css("position", "relative");
			} else {
				$('footer').css("position", "fixed");
			}
		} else {
			scrollTop = $(window).scrollTop();
			if (scrollTop > 0) {
				$('footer').css("position", "relative");
			} else {
				$('footer').css("position", "fixed");
			}
		}
	},

	init: function () {
		this.setFooter();
		this.setImageFieldHandler();
		this.applyCss();
		this.initPlugins();

		this.bodyHeight = $("body").height();

		this.initEventHandlers();
	},

	initEventHandlers: function () {
		var that = this;
		$(document).on('click', function () {
			that.countTrying = 0;
			that.refreshInterval = setInterval("shared.bodyHeightChanged();", 50);
		});

		$(document).ajaxError(function (e, jqxhr, settings) {
			e.stopPropagation();
			var response = { message: "В результате выполнения запроса возникла непредвиденная ошибка. Обновите страницу и свяжитесь с администратором для выявления деталей." };
			if (jqxhr.responseText != null) {
				response = JSON.parse(jqxhr.responseText);
			}

			bootbox.dialog(response.message, [{
				"label": "ОК",
				"class": "btn btn-primary",
			}], { "header": "Ошибка в результате запроса." });
		});

		that.setImageFieldHandler();
	},

	applyCss: function () {
		var addButtons = $(".addItemButton");
		addButtons.html("<i class=\"icon-black icon-plus\"></i>");
		addButtons.addClass("btn btn-mini");

		var editButtons = $(".editItemButton");
		editButtons.html("<i class=\"icon-black icon-edit\"></i>");
		editButtons.addClass("btn btn-mini");

		var removeButtons = $(".removeItemButton");
		removeButtons.html("<i class=\"icon-black icon-remove\"></i>");
		removeButtons.addClass("btn btn-mini");

		var removeLookupButtons = $(".removeLookupButton");
		removeLookupButtons.html("<i class=\"icon-black icon-remove\"></i>");
		removeLookupButtons.addClass("btn btn-mini");

		$('.alert-error,.alert-success').addCloseButtonInAlert();
	},

	errorAlert: function () {
		$('.alert-error').show();
	},

	setImageFieldHandler: function () {
		var that = this;
		var itemFields = $('img.upload-view');
		$.map(itemFields, function (e) {
			if ($(e).attr('src') === undefined || $(e).attr('src') === "#") {
				$(e).attr('src', '/Images/noPhoto.png');
			}
		});

		$("input.upload-view").handle('change', function () {
			that.getFileContent(this);
		});
	},

	getFileContent: function (input) {
		if ($.isIE()) {
			try {
				var fileSystemObject = new ActiveXObject('Scripting.FileSystemObject');
				var file = fileSystemObject.getFile(input.value);

				$('#fileName_' + input.id.split('file_')[1]).text(file.Path);
				$('#image-field_' + input.id.split('file_')[1]).remove();

			} catch (ex) {
				alert('Скорее всего у вас не настроена работа ActiveXObject. Проверьте настройки браузера.');
			}
		} else {
			if (input.files && input.files[0]) {
				var reader = new FileReader();
				reader.onload = function (e) {
					$('img.upload-view').attr('src', e.target.result);
				};
				reader.readAsDataURL(input.files[0]);
			}
		}
	},

	initPlugins: function (container) {
		if (container == null) {
			container = $('body');
		}
		container.find(".datetimepicker").each(function (index, value) {
			var dateFormat = "dd.mm.yyyy";
			var viewMode = dateFormat == "yyyy" ? "years" : "days";

			$(value).datepicker({
				format: dateFormat,
				language: "ru-Ru",
				autoclose: true,
				viewMode: viewMode,
				minViewMode: viewMode
			});
		});

		container.find("select").each(function (index, element) {
			$(element).multiselect({
				buttonClass: 'btn btn-primary',
				buttonWidth: '500',
				maxHeight: false,
				selectAllText: true,
				buttonText: function (options) {
					if (options.length == 0) {
						return "Ничего не выбрано <b class=\"caret\"></b>";
					} else if (options.length > 3) {
						return options.length + " Выбрано  <b class=\"caret\"></b>";
					} else {
						var selected = '';
						options.each(function () {
							selected += $(this).text() + ', ';
						});
						return selected.substr(0, selected.length - 2) + " <b class=\"caret\"></b>";
					}
				}
			});
		});

		container.find("select.editable").each(function (index, element) {
			$(element).editabledropdown();
		});

		container.find(".singleselect").on("change", function () {
			var selectedValue = $(this).find("option:selected").val();
			$(this).find("option").attr("selected", false);
			$(this).find('option[value="' + selectedValue + '"]').attr("selected", true);
		});
	},

	bodyHeightChanged: function () {
		var currentHeight = $("body").height();

		if (currentHeight != shared.bodyHeight) {
			this.setFooter();
			clearInterval(shared.refreshInterval);
			this.bodyHeight = currentHeight;
			this.countTrying = 0;
		} else {
			this.countTrying++;
			if (this.countTrying == 10) {
				clearInterval(shared.refreshInterval);
				this.countTrying = 0;
			}
		}
	}
};

$(document).ready(function () {
	shared.init();

	$('input[type=text],textarea,select').each(function (index, element) {
		var req = $(this).attr('data-val-required');
		if (undefined != req) {
			var label = $('label[for="' + $(this).attr('id') + '"]');
			var text = label.text();
			if (text.length > 0) {
				var btn = $(element).next();
			}
		}

	});

	$(document).ready(function () {
	});
});