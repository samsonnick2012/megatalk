using System.ComponentModel.DataAnnotations;

namespace Innostar.UI.Areas.Administration.ViewModels.AccountViewModels
{
	public class LocalPasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "������� ������", Description = "�������� �������� ������")]
		public string OldPassword
		{
			get;
			set;
		}

		[Required]
		[StringLength(100, ErrorMessage = "{0} ������ ���� �� ����� {2} ��������.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "����� ������", Description = "����� ������ �� ��������")]
		public string NewPassword
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "����������� ����� ������", Description = "������������� ������ ������ �� ��������")]
		[Compare("NewPassword", ErrorMessage = "������ �� ���������.")]
		public string ConfirmPassword
		{
			get;
			set;
		}

		[DataType(DataType.EmailAddress)]
		[Display(Name = "�������� �����")]
		[StringLength(50, ErrorMessage = "Email �� ����� ����� ������ ������ 50 ��������")]
		public string Email
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "��� �� ����� ����� ������ ������ 50 ��������")]
		[DataType(DataType.Text)]
		[Display(Name = "���")]
		public string Name
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "������� �� ����� ����� ������ ������ 50 ��������")]
		[DataType(DataType.Text)]
		[Display(Name = "�������")]
		public string Surname
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "�������� �� ����� ����� ������ ������ 50 ��������")]
		[DataType(DataType.Text)]
		[Display(Name = "��������")]
		public string Patronymic
		{
			get;
			set;
		}

		[StringLength(50, ErrorMessage = "������� �� ����� ����� ������ ������ 50 ��������")]
		[DataType(DataType.PhoneNumber)]
		[Display(Name = "�������")]
		public string PhoneNumber
		{
			get;
			set;
		}
	}
}