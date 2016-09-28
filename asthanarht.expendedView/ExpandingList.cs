using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace asthanarht.expendedView
{
    public class ExpandingList :ScrollView
    {
        private LinearLayout mContainer;

        public ExpandingList(Context context, IAttributeSet attrs) :base(context,attrs)
        {
            mContainer = new LinearLayout(context);
            mContainer.Orientation = Orientation.Vertical;
            mContainer.AddView(mContainer);
        }

        /**
    * Method to add a new item.
    * @param item The ExpandingItem item.
    */
        private void AddItem(ExpandingItem item)
        {
            mContainer.AddView(item);
        }

        /**
         * Method to create and add a new item.
         * @param layoutId The item Layout.
         * @return The created item.
         */
        public ExpandingItem createNewItem(int layoutId)
        {
            LayoutInflater inflater = LayoutInflater.From(Context);
            ViewGroup item = (ViewGroup)inflater.Inflate(layoutId, this, false);
            if (item is ExpandingItem) {
                ExpandingItem expandingItem = (ExpandingItem)item;
                //expandingItem.Parent(this);
                AddItem(expandingItem);
                return expandingItem;
            }
            throw new Exception("The layout id not an instance of com.diegodobelo.expandinganimlib.ExpandingItem");
        }

        /**
         * Method to remove an item.
         * @param item The item to be removed.
         */
        public void RemoveItem( ExpandingItem item)
        {
            mContainer.RemoveView(item);
        }


        /**
         * Method to remove all items.
         */
        public void RemoveAllViews()
        {
            mContainer.RemoveAllViews();
        }

        /**
         * Scroll up to show sub items
         * @param delta The calculated amount to scroll up.
         */
        protected void scrollUpByDelta( int delta)
        {
            Post(() =>
            {
                SmoothScrollTo(0, ScrollY + delta);
            });

         }
    }
}