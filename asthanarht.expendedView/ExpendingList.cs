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
    public class ExpendingList :ScrollView
    {
        private LinearLayout mContainer;

        public ExpendingList(Context context, IAttributeSet attrs) :base(context,attrs)
        {
            mContainer = new LinearLayout(context);
            mContainer.Orientation = Orientation.Vertical;
            mContainer.AddView(mContainer);
        }

    }
}