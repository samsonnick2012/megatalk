namespace Application.Core.UI.ViewModels
{
	public abstract class ModelViewModelBase<TModel> : ViewModelBase where TModel : new()
	{
		public TModel DomainModel
		{
			get { return ConvertToDomainModel(); }
		}

		protected abstract TModel ConvertToDomainModel();

		public int Id
		{
			get; set;
		}
	}
}
