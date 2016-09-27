using Android.Views;

namespace asthanarht.expendedView.Util
{
    public  class CustomViewUtil
    {
        public static void SetViewHeight(View v, int height)
        {
             ViewGroup.LayoutParams param = v.LayoutParameters;
              param.Height = height;
               v.RequestLayout();
        }

        
        public static void SetViewWidth(View v, int width)
        {
             ViewGroup.LayoutParams param = v.LayoutParameters;
             param.Width = width;
             v.RequestLayout();
        }

        public static void SetViewMarginTop(View v, int marginTop)
        {
            SetViewMargin(v, 0, marginTop, 0, 0);
        }

        public static void SetViewMargin(View v, int left, int top, int right, int bottom)
        {
            ViewGroup.MarginLayoutParams param = (ViewGroup.MarginLayoutParams)v.LayoutParameters;
             param.SetMargins(left, top, right, bottom);
             v.RequestLayout();
        }
    }
}