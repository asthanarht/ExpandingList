using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using asthanarht.expendedView.Util;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Android.Graphics.Drawables;
using Android.Views.Animations;
using Java.Lang;
using asthanarht.expendedView;
using Android.Graphics;

namespace asthanarht.expendedView
{
    public class ExpandingItem : RelativeLayout
    {
        private static int DEFAULT_ANIM_DURATION = 300;

        /**
         * Member variable to hold the Item Layout. Set by item_layout in ExpandingItem layout.
         */
        private ViewGroup mItemLayout;

        /**
         * The layout inflater.
         */
        private LayoutInflater mInflater;

        /**
         * Member variable to hold the base layout. Should not be changed.
         */
        private RelativeLayout mBaseLayout;

        /**
         * Member variable to hold item. Should not be changed.
         */
        private LinearLayout mBaseListLayout;

        /**
         * Member variable to hold sub items. Should not be changed.
         */
        private LinearLayout mBaseSubListLayout;

        /**
         * Member variable to hold the indicator icon.
         * Can be set by {@link #setIndicatorIconRes(int)}} or by {@link #setIndicatorIcon(Drawable)}.
         */
        private ImageView mIndicatorImage;

        /**
         * Member variable to hold the expandable part of indicator. Should not be changed.
         */
        private View mIndicatorBackground;

        /**
         * Stub to hold separator;
         */
        private ViewStub mSeparatorStub;

        /**
         * Member variable to hold the indicator container. Should not be changed.
         */
        private ViewGroup mIndicatorContainer;

        /**
         * Member variable to hold the measured item height.
         */
        private int mItemHeight;

        /**
         * Member variable to hold the measured sub item height.
         */
        private int mSubItemHeight;

        /**
         * Member variable to hold the measured sub item width.
         */
        private int mSubItemWidth;

        /**
         * Member variable to hold the measured total height of sub items.
         */
        private int mCurrentSubItemsHeight;

        /**
         * Member variable to hold the sub items count.
         */
        private int mSubItemCount;

        /**
         * Member variable to hold the indicator size. Set by indicator_size in ExpandingItem layout.
         */
        private int mIndicatorSize;

        /**
         * Member variable to hold the animation duration.
         * Set by animation_duration in ExpandingItem layout in milliseconds.
         * Default is 300ms.
         */
        private int mAnimationDuration;

        /**
         * Member variable to hold the indicator margin at left. Set by indicator_margin_left in ExpandingItem layout.
         */
        private int mIndicatorMarginLeft;

        /**
         * Member variable to hold the indicator margin at right. Set by indicator_margin_right in ExpandingItem layout.
         */
        private int mIndicatorMarginRight;

        /**
         * Member variable to hold the boolean value that defines if the indicator should be shown.
         * Set by show_indicator in ExpandingItem layout.
         */
        private bool mShowIndicator;

        /**
         * Member variable to hold the boolean value that defines if the animation should be shown.
         * Set by show_animation in ExpandingItem layout. Default is true.
         */
        private bool mShowAnimation;

        /**
         * Member variable to hold the boolean value that defines if the sub list will start collapsed or not.
         * Set by start_collapsed in ExpandingItem layout. Default is true.
         */
        private bool mStartCollapsed;

        /**
         * Member variable to hold the state of sub items. true if shown. false otherwise.
         */
        private bool mSubItemsShown;

        /**
         * Member variable to hold the layout resource of sub items. Set by sub_item_layout in ExpandingItem layout.
         */
        private int mSubItemLayoutId;

        /**
         * Member variable to hold the layout resource of items. Set by item_layout in ExpandingItem layout.
         */
        private int mItemLayoutId;

        /**
         * Member variable to hold the layout resource of separator. Set by separator_layout in ExpandingItem layout.
         */
        private int mSeparatorLayoutId;

        /**
         * Holds a reference to the parent. Used to calculate positioning.
         */
        private ExpendingList mParent;

        /**
         * Member variable to hold the listener of item state change.
         */
        private OnItemStateChanged mListener;

        /**
         * Interface to notify item state changed.
         */

        public interface OnItemStateChanged
        {
            /**
             * Notify if item was expanded or collapsed.
             * @param expanded true if expanded. false otherwise.
             */
            void itemCollapseStateChanged(bool expanded);
        }

        /**
         * Constructor.
         * @param context
         * @param attrs
         */

        public ExpandingItem(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            ReadAttributes(context, attrs);
            SetupStateVariables();
            InflateLayouts(context);

            SetupIndicator();

            AddItem(mItemLayout);
            AddView(mBaseLayout);
        }

        /**
         * Setup the variables that defines item state.
         */

        private void SetupStateVariables()
        {
            if (!mShowAnimation)
            {
                mAnimationDuration = 0;
            }
        }

        /**
         * Method to setup indicator, including size and visibility.
         */

        private void SetupIndicator()
        {
            if (mIndicatorSize != 0)
            {
                SetIndicatorBackgroundSize();
            }

            mIndicatorContainer.Visibility = (mShowIndicator && mIndicatorSize != 0 ? ViewStates.Visible : ViewStates.Gone);
        }

        /**
         * Read all custom styleable attributes.
         * @param context The custom View Context.
         * @param attrs The atrributes to be read.
         */

        private void ReadAttributes(Context context, IAttributeSet attrs)
        {
            TypedArray array = context.Theme.ObtainStyledAttributes(attrs,
                Resource.Styleable.ExpandingItem, 0, 0);

            try
            {
                mItemLayoutId = array.GetResourceId(Resource.Styleable.ExpandingItem_item_layout, 0);
                mSeparatorLayoutId = array.GetResourceId(Resource.Styleable.ExpandingItem_separator_layout, 0);
                mSubItemLayoutId = array.GetResourceId(Resource.Styleable.ExpandingItem_sub_item_layout, 0);
                mIndicatorSize = array.GetDimensionPixelSize(Resource.Styleable.ExpandingItem_indicator_size, 0);
                mIndicatorMarginLeft =
                    array.GetDimensionPixelSize(Resource.Styleable.ExpandingItem_indicator_margin_left, 0);
                mIndicatorMarginRight =
                    array.GetDimensionPixelSize(Resource.Styleable.ExpandingItem_indicator_margin_right, 0);
                mShowIndicator = array.GetBoolean(Resource.Styleable.ExpandingItem_show_indicator, true);
                mShowAnimation = array.GetBoolean(Resource.Styleable.ExpandingItem_show_animation, true);
                mStartCollapsed = array.GetBoolean(Resource.Styleable.ExpandingItem_start_collapsed, true);
                mAnimationDuration = array.GetInt(Resource.Styleable.ExpandingItem_animation_duration,
                    DEFAULT_ANIM_DURATION);
            }
            finally
            {
                array.Recycle();
            }
        }

        /**
         * Method to inflate all layouts.
         * @param context The custom View Context.
         */

        private void InflateLayouts(Context context)
        {
            mInflater = LayoutInflater.From(context);
            mBaseLayout = (RelativeLayout) mInflater.Inflate(Resource.Layout.expanding_item_base_layout,
                null, false);
            mBaseListLayout = (LinearLayout) mBaseLayout.FindViewById(Resource.Id.base_list_layout);
            mBaseSubListLayout = (LinearLayout) mBaseLayout.FindViewById(Resource.Id.base_sub_list_layout);
            mIndicatorImage = (ImageView) mBaseLayout.FindViewById(Resource.Id.indicator_image);
            mBaseLayout.FindViewById(Resource.Id.icon_indicator_top).BringToFront();
            mSeparatorStub = (ViewStub) mBaseLayout.FindViewById(Resource.Id.base_separator_stub);
            mIndicatorBackground = mBaseLayout.FindViewById(Resource.Id.icon_indicator_middle);
            mIndicatorContainer = (ViewGroup) mBaseLayout.FindViewById(Resource.Id.indicator_container);
            mIndicatorContainer.Click += delegate
            {
                ToggleExpanded();
            };

            if (mItemLayoutId != 0)
            {
                mItemLayout = (ViewGroup) mInflater.Inflate(mItemLayoutId, mBaseLayout, false);
            }
            if (mSeparatorLayoutId != 0)
            {
                mSeparatorStub.LayoutResource = mSeparatorLayoutId;
                mSeparatorStub.Inflate();
            }
        }

        /**
     * Set the indicator background width, height and margins
     */

        private void SetIndicatorBackgroundSize()
        {
            CustomViewUtil.SetViewHeight(mBaseLayout.FindViewById(Resource.Id.icon_indicator_top), mIndicatorSize);
            CustomViewUtil.SetViewHeight(mBaseLayout.FindViewById(Resource.Id.icon_indicator_bottom), mIndicatorSize);
            CustomViewUtil.SetViewHeight(mBaseLayout.FindViewById(Resource.Id.icon_indicator_middle), 0);

            CustomViewUtil.SetViewWidth(mBaseLayout.FindViewById(Resource.Id.icon_indicator_top), mIndicatorSize);
            CustomViewUtil.SetViewWidth(mBaseLayout.FindViewById(Resource.Id.icon_indicator_bottom), mIndicatorSize);
            CustomViewUtil.SetViewWidth(mBaseLayout.FindViewById(Resource.Id.icon_indicator_middle), mIndicatorSize);

//    mItemLayout.Post(new Runnable() {

//            public void run()
//{
//    CustomViewUtils.setViewMargin(mIndicatorContainer,
//            mIndicatorMarginLeft, mItemLayout.GetMeasuredHeight() / 2 - mIndicatorSize / 2, mIndicatorMarginRight, 0);
//}
//        });

            CustomViewUtil.SetViewMarginTop(mBaseLayout.FindViewById(Resource.Id.icon_indicator_middle),
                (-1*mIndicatorSize/2));
            CustomViewUtil.SetViewMarginTop(mBaseLayout.FindViewById(Resource.Id.icon_indicator_bottom),
                (-1*mIndicatorSize/2));

        }

        /**
     * Set a listener to listen item stage changed.
     * @param listener The listener of type {@link OnItemStateChanged}
     */

        public void SetStateChangedListener(OnItemStateChanged listener)
        {
            mListener = listener;
        }

/**
 * Tells if the item is expanded.
 * @return true if expanded. false otherwise.
 */

        public bool IsExpanded()
        {
            return mSubItemsShown;
        }

/**
 * Returns the count of sub items.
 * @return The count of sub items.
 */

        public int GetSubItemsCount()
        {
            return mSubItemCount;
        }

/**
 * Collapses the sub items.
 */

        public void Collapse()
        {
            mSubItemsShown = false;
//    mBaseSubListLayout.post(new Runnable() {
//            @Override
//            public void run()
//{
//    CustomViewUtils.setViewHeight(mBaseSubListLayout, 0);
//}
//        });
        }

        /**
     * Expand or collapse the sub items.
     */

        public void ToggleExpanded()
        {
            if (mSubItemCount == 0)
            {
                return;
            }

            if (!mSubItemsShown)
            {
                AdjustItemPosIfHidden();
            }

            ToggleSubItems();
            ExpandSubItemsWithAnimation(0f);
            ExpandIconIndicator(0f);
            AnimateSubItemsIn();
        }

/**
 * Method to adjust Item position in parent if its sub items are outside screen.
 */

        private void AdjustItemPosIfHidden()
        {
            int parentHeight = mParent.Height;
            int[] parentPos = new int[2];
            mParent.GetLocationOnScreen(parentPos);
            int parentY = parentPos[1];
            int[] itemPos = new int[2];
            mBaseLayout.GetLocationOnScreen(itemPos);
            int itemY = itemPos[1];


            int endPosition = itemY + mItemHeight + (mSubItemHeight*mSubItemCount);
            int parentEnd = parentY + parentHeight;
            if (endPosition > parentEnd)
            {
                int delta = endPosition - parentEnd;
                int itemDeltaToTop = itemY - parentY;
                if (delta > itemDeltaToTop)
                {
                    delta = itemDeltaToTop;
                }
                mParent.ScrollY = delta;
               // mParent.ScrollUpByDelta(delta);
            }
        }

/**
 * Set the indicator color by resource.
 * @param colorRes The color resource.
 */

        public void SetIndicatorColorRes(int colorRes)
        {
            //SetIndicatorColor( ContextCompat.GetColor(Context(), colorRes));
        }

/**
 * Set the indicator color by color value.
 * @param color The color value.
 */

        public void SetIndicatorColor(Color color)
        {
            ((GradientDrawable) FindViewById(Resource.Id.icon_indicator_top).Background.Mutate()).SetColor(color);
            ((GradientDrawable) FindViewById(Resource.Id.icon_indicator_bottom).Background.Mutate()).SetColor(color);
            FindViewById(Resource.Id.icon_indicator_middle).SetBackgroundColor( color);
        }

/**
 * Set the indicator icon by resource.
 * @param iconRes The icon resource.
 */

        public void SetIndicatorIconRes(int iconRes)
        {
            //SetIndicatorIcon(ContextCompat.getDrawable(getContext(), iconRes));
        }

/**
 * Set the indicator icon.
 * @param icon Drawable of the indicator icon.
 */

        public void SetIndicatorIcon(Drawable icon)
        {
            mIndicatorImage.SetImageDrawable(icon);
        }


        public View CreateSubItem()
        {
            return CreateSubItem(-1);
        }

/**
 * Creates a sub item based on sub_item_layout Layout, set as ExpandingItem layout attribute.
 * @param position The position to add the new Item. Position should not be greater than the list size.
 *                 If position is -1, the item will be added in the end of the list.
 * @return The inflated sub item view.
 */

        public View CreateSubItem(int position)
        {
            if (mSubItemLayoutId == 0)
            {
                throw new RuntimeException("There is no layout to be inflated. " +
                                           "Please set sub_item_layout value");
            }

            if (position > mBaseSubListLayout.ChildCount)
            {
                throw new IllegalArgumentException("Cannot add an item at position " + position +
                                                   ". List size is " + mBaseSubListLayout.ChildCount);
            }

            ViewGroup subItemLayout = (ViewGroup) mInflater.Inflate(mSubItemLayoutId, mBaseSubListLayout, false);
            if (position == -1)
            {
                mBaseSubListLayout.AddView(subItemLayout);
            }
            else
            {
                mBaseSubListLayout.AddView(subItemLayout, position);
            }
            mSubItemCount++;
            SetSubItemDimensions(subItemLayout);

            //Animate sub view in
            if (mSubItemsShown)
            {
                CustomViewUtil.SetViewHeight(subItemLayout, 0);
                ExpandSubItemsWithAnimation(mSubItemHeight*(mSubItemCount));
                ExpandIconIndicator(mCurrentSubItemsHeight);
                AnimateSubItemAppearance(subItemLayout, true);
                AdjustItemPosIfHidden();
            }

            return subItemLayout;
        }

/**
 * Creates as many sub items as requested in {@param count}.
 * @param count The quantity of sub items.
 */

        public void CreateSubItems(int count)
        {
            if (mSubItemLayoutId == 0)
            {
                throw new RuntimeException("There is no layout to be inflated. " +
                                           "Please set sub_item_layout value");
            }
            for (int i = 0; i < count; i++)
            {
                CreateSubItem();
            }
            if (mStartCollapsed)
            {
                Collapse();
            }
            else
            {
                mSubItemsShown = true;
//        mBaseSubListLayout.post(new Runnable() {
//                @Override
//                public void run()
//{
//    expandIconIndicator(0f);
//}
//            });
            }
        }

        /**
     * Get a sub item at the given position.
     * @param position The sub item position. Should be > 0.
     * @return The sub item inflated view at the given position.
     */

        public View getSubItemView(int position)
        {
            if (mBaseSubListLayout.GetChildAt(position) != null)
            {
                return mBaseSubListLayout.GetChildAt(position);
            }
            throw new RuntimeException("There is no sub item for position " + position +
                                       ". There are only " + mBaseSubListLayout.ChildCount + " in the list.");
        }

/**
 * Remove sub item at the given position.
 * @param position The position of the item to be removed.
 */

        public void RemoveSubItemAt(int position)
        {
            RemoveSubItemFromList(mBaseSubListLayout.GetChildAt(position));
        }

/**
 * Remove the given view representing the sub item. Should be an existing sub item.
 * @param view The sub item to be removed.
 */

        public void RemoveSubItemFromList(View view)
        {
            if (view != null)
            {
                mBaseSubListLayout.RemoveView(view);
                mSubItemCount--;
                ExpandSubItemsWithAnimation(mSubItemHeight*(mSubItemCount + 1));
                if (mSubItemCount == 0)
                {
                    mCurrentSubItemsHeight = 0;
                    mSubItemsShown = false;
                }
                ExpandIconIndicator(mCurrentSubItemsHeight);
            }
        }

/**
 * Remove the given view representing the sub item, with animation. Should be an existing sub item.
 * @param view The sub item to be removed.
 */

        public void RemoveSubItem(View view)
        {
            AnimateSubItemAppearance(view, false);
        }

/**
 * Remove all sub items.
 */

        public void RemoveAllSubItems()
        {
            mBaseSubListLayout.RemoveAllViews();
        }

/**
 * Set the parent in order to auto scroll.
 * @param parent The parent of type {@link ExpandingList}
 */

        protected void SetParent(ExpendingList parent)
        {
            mParent = parent;
        }

/**
 * Method to add the inflated item and set click listener.
 * Also measures the item height.
 * @param item The inflated item layout.
 */

        private void AddItem(ViewGroup item)
        {
            if (item != null)
            {
                mBaseListLayout.AddView(item);
                item.Click += delegate
                {
                    ToggleExpanded();
                };

            }
            

//            item.post(new Runnable()
//{
//    @Override
//                public void run()
//{
//    mItemHeight = item.getMeasuredHeight();
//}
//            });
        }


        /**
     * Measure sub items dimension.
     * @param v The sub item to measure.
     */

        private void SetSubItemDimensions(ViewGroup v)
        {
//    v.post(new Runnable() {
//            @Override
//            public void run()
//{
//    if (mSubItemHeight <= 0)
//    {
//        mSubItemHeight = v.getMeasuredHeight();
//        mSubItemWidth = v.getMeasuredWidth();
//    }
//}
//        });
        }

        /**
     * Toggle sub items collapsed/expanded
     */

        private void ToggleSubItems()
        {
            mSubItemsShown = !mSubItemsShown;
            if (mListener != null)
            {
                mListener.itemCollapseStateChanged(mSubItemsShown);
            }
        }

/**
 * Show sub items animation.
 */

        private void AnimateSubItemsIn()
        {
            for (int i = 0; i < mSubItemCount; i++)
            {
                AnimateSubViews((ViewGroup) mBaseSubListLayout.GetChildAt(i), i);
                AnimateViewAlpha((ViewGroup) mBaseSubListLayout.GetChildAt(i), i);
            }
        }

/**
 * Show sub items translation animation.
 * @param viewGroup The sub item to animate
 * @param index The sub item index. Needed to calculate delays.
 */

        private void AnimateSubViews(ViewGroup viewGroup, int index)
        {
            if (viewGroup == null)
            {
                return;
            }
            viewGroup.SetLayerType(LayerType.Hardware, null);
            ValueAnimator animation = mSubItemsShown
                ? ValueAnimator.OfFloat(0f, 1f)
                : ValueAnimator.OfFloat(1f, 0f);
            animation.SetDuration(mAnimationDuration);
            int delay = index*mAnimationDuration/mSubItemCount;
            int invertedDelay = (mSubItemCount - index)*mAnimationDuration/mSubItemCount;
            animation.StartDelay = (mSubItemsShown ? delay/2 : invertedDelay/2);

            animation.Update += delegate
            {
                float val = (float) animation.AnimatedValue;
                viewGroup.SetX((mSubItemWidth/2*val) - mSubItemWidth/2);
            };

            animation.AnimationEnd += delegate
            {
                viewGroup.SetLayerType(LayerType.Hardware, null);
            };


            animation.Start();
        }

        /**
     * Show sub items alpha animation.
     * @param viewGroup The sub item to animate
     * @param index The sub item index. Needed to calculate delays.
     */

        private void AnimateViewAlpha(ViewGroup viewGroup, int index)
        {
            if (viewGroup == null)
            {
                return;
            }
            ValueAnimator animation = mSubItemsShown
                ? ValueAnimator.OfFloat(0f, 1f)
                : ValueAnimator.OfFloat(1f, 0f);
            animation.SetDuration(mSubItemsShown ? mAnimationDuration*2 : mAnimationDuration);
            int delay = index*mAnimationDuration/mSubItemCount;
            animation.StartDelay = mSubItemsShown ? delay/2 : 0;

            animation.Update += delegate
            {
                float val = (float) animation.AnimatedValue;
                viewGroup.Alpha = val;
            };

            animation.Start();
        }

        /**
     * Show indicator animation.
     * @param startingPos The position from where the animation should start. Useful when removing sub items.
     */

        private void ExpandIconIndicator(float startingPos)
        {
            if (mIndicatorBackground != null)
            {
                int totalHeight = (mSubItemHeight*mSubItemCount) - mIndicatorSize/2 + mItemHeight/2;
                mCurrentSubItemsHeight = totalHeight;
                ValueAnimator animation = mSubItemsShown
                    ? ValueAnimator.OfFloat(startingPos, totalHeight)
                    : ValueAnimator.OfFloat(totalHeight, startingPos);
                animation.SetInterpolator(new AccelerateDecelerateInterpolator());
                animation.SetDuration(mAnimationDuration);
                animation.Update += delegate
                {
                    float val = (float) animation.AnimatedValue;
                    CustomViewUtil.SetViewHeight(mIndicatorBackground, (int) val);
                };


                animation.Start();
            }
        }

        /**
     * Expand the sub items container with animation
     * @param startingPos The position from where the animation should start. Useful when removing sub items.
     */

        private void ExpandSubItemsWithAnimation(float startingPos)
        {
            if (mBaseSubListLayout != null)
            {
                int totalHeight = (mSubItemHeight*mSubItemCount);
                ValueAnimator animation = mSubItemsShown
                    ? ValueAnimator.OfFloat(startingPos, totalHeight)
                    : ValueAnimator.OfFloat(totalHeight, startingPos);
                animation.SetInterpolator(new AccelerateDecelerateInterpolator());
                animation.SetDuration(mAnimationDuration);
                animation.Update += delegate
                {
                    float val = (float) animation.AnimatedValue;
                    CustomViewUtil.SetViewHeight(mBaseSubListLayout, (int) val);
                };
                animation.AnimationEnd += delegate
                {
                    if (mSubItemsShown)
                    {
                        AdjustItemPosIfHidden();
                    }
                };




                animation.Start();
            }
        }

        /**
     * Remove the given sub item after animation ends.
     * @param subItem The view representing the sub item to be removed.
     * @param isAdding true if adding a view. false otherwise.
     */

        private void AnimateSubItemAppearance(View subItem, bool isAdding)
        {
            ValueAnimator alphaAnimation = isAdding
                ? ValueAnimator.OfFloat(0f, 1f)
                : ValueAnimator.OfFloat(1f, 0f);
            alphaAnimation.SetDuration(isAdding ? mAnimationDuration*2 : mAnimationDuration/2);

            ValueAnimator heightAnimation = isAdding
                ? ValueAnimator.OfFloat(0f, mSubItemHeight)
                : ValueAnimator.OfFloat(mSubItemHeight, 0f);
            heightAnimation.SetDuration(mAnimationDuration/2);
            heightAnimation.StartDelay = (mAnimationDuration/2);
            alphaAnimation.Update += delegate
            {
                float val = (float) alphaAnimation.AnimatedValue;
                subItem.Alpha = (val);
            };
            heightAnimation.Update += delegate
            {
                float val = (float) heightAnimation.AnimatedValue;
                CustomViewUtil.SetViewHeight(subItem, (int) val);
            };




            alphaAnimation.Start();
            heightAnimation.Start();

            if (!isAdding)
            {
                heightAnimation.AnimationEnd += delegate
                {



                    RemoveSubItemFromList(subItem);

                };
            }
        }
    }
}