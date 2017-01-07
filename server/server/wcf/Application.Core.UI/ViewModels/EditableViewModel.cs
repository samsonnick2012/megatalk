namespace Application.Core.UI.ViewModels
{
	public abstract class EditableViewModel<TModel> : ModelViewModelBase<TModel> where TModel : new()
	{
		public ViewModelChangeAction ViewModelChangeAction
		{
			get;
			set;
		}
	}
}
