﻿using System;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[RenderWith(typeof(_CarouselViewRenderer))]
	public class CarouselView : ItemsView, ICarouselViewController
	{
		public static readonly BindableProperty PositionProperty = 
			BindableProperty.Create(
				propertyName: nameof(Position), 
				returnType: typeof(int), 
				declaringType: typeof(CarouselView), 
				defaultValue: 0, 
				defaultBindingMode: BindingMode.TwoWay
			);

		public static readonly BindableProperty ItemProperty = 
			BindableProperty.Create(
				propertyName: nameof(Item), 
				returnType: typeof(object), 
				declaringType: typeof(CarouselView), 
				defaultValue: null, 
				defaultBindingMode: BindingMode.TwoWay
			);

		#region Fields
		object _lastItem;
		int _lastPosition;
		#endregion

		public CarouselView()
		{
			_lastPosition = 0;
			_lastItem = null;
			VerticalOptions = LayoutOptions.FillAndExpand;
			HorizontalOptions = LayoutOptions.FillAndExpand;
		}

		#region Private Members
		object GetItem(int position)
		{
			var controller = (IItemViewController)this;
			object item = controller.GetItem(position);
			return item;
		}
		#endregion

		// non-public bc unable to implement on iOS
		internal event EventHandler<ItemVisibilityEventArgs> ItemAppearing;
		internal event EventHandler<ItemVisibilityEventArgs> ItemDisappearing;

		public int Position
		{
			get { return (int)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		public int Item
		{
			get { return (int)GetValue(ItemProperty); }
		}

		public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
		public event EventHandler<SelectedPositionChangedEventArgs> PositionSelected;

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var minimumSize = new Size(40, 40);
			return new SizeRequest(minimumSize, minimumSize);
		}

		void ICarouselViewController.SendPositionAppearing(int position)
		{
			ItemAppearing?.Invoke(this, new ItemVisibilityEventArgs(GetItem(position)));
		}
		void ICarouselViewController.SendPositionDisappearing(int position)
		{
			ItemDisappearing?.Invoke(this, new ItemVisibilityEventArgs(GetItem(position)));
		}
		void ICarouselViewController.SendSelectedItemChanged(object item)
		{
			if (item.Equals(_lastItem))
				return;

			ItemSelected?.Invoke(this, new SelectedItemChangedEventArgs(item));
			_lastItem = item;
		}
		void ICarouselViewController.SendSelectedPositionChanged(int position)
		{
			if (_lastPosition == position)
				return;

			_lastPosition = position;
			PositionSelected?.Invoke(this, new SelectedPositionChangedEventArgs(position));
		}
	}
}