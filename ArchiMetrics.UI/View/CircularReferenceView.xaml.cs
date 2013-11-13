// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularReferenceView.xaml.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Interaction logic for CircularReferenceView.xaml.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.View
{
	using System.Windows.Controls;
	using ArchiMetrics.UI.Support;
	using ArchiMetrics.UI.ViewModel;

	/// <summary>
	/// Interaction logic for CircularReferenceView.xaml.
	/// </summary>
	[DataContext(typeof(CircularReferenceViewModel))]
	public partial class CircularReferenceView : UserControl
	{
		public CircularReferenceView()
		{
			InitializeComponent();
		}
	}
}
