using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.Workspace.Shared
{
    public class PageIndexChangeEventArgs : EventArgs
    {
        public int OldPageIndex { get; private set; }

        public int NewPageIndex { get; private set; }

        public PageIndexChangeEventArgs(int oldPageIndex, int newPageIndex)
        {
            OldPageIndex = oldPageIndex;
            NewPageIndex = newPageIndex;
        }
    }

    public class PagerViewModel : PropertyChangedBase
    {
        public event EventHandler<PageIndexChangeEventArgs> PageIndexChanged;

        private int _pageSize;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            private set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    NotifyOfPropertyChange(() => PageSize);

                    if (PageIndex > TotalPages - 1)
                    {
                        PageIndex = TotalPages - 1;
                    }
                }
            }
        }

        private int _pageIndex;

        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (_pageIndex != value)
                {
                    var oldPageIndex = _pageIndex;

                    _pageIndex = value;
                    NotifyOfPropertyChange(() => PageIndex);
                    NotifyOfPropertyChange(() => HasPrev);
                    NotifyOfPropertyChange(() => HasNext);

                    OnPageIndexChanged(oldPageIndex, _pageIndex);
                }
            }
        }

        public int PageNumber
        {
            get
            {
                return PageIndex + 1;
            }
        }

        public Visibility Visibility
        {
            get
            {
                return TotalPages > 1 ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public int TotalPages
        {
            get
            {
                return TotalRecords / PageSize + (TotalRecords % PageSize > 0 ? 1 : 0);
            }
        }

        public bool HasNext
        {
            get
            {
                return PageIndex < TotalPages - 1;
            }
        }

        public bool HasPrev
        {
            get
            {
                return PageIndex > 0;
            }
        }

        private int _totalRecords;

        public int TotalRecords
        {
            get
            {
                return _totalRecords;
            }
            private set
            {
                if (_totalRecords != value)
                {
                    _totalRecords = value;
                    NotifyOfPropertyChange(() => TotalRecords);
                }
            }
        }

        public PagerViewModel(int pageSize)
        {
            PageSize = pageSize;
        }

        public void Bind<T>(PagedQueryable<T> query, int pageIndex)
        {
            TotalRecords = query.Count;
            PageIndex = pageIndex;

            NotifyOfPropertyChange(() => TotalPages);
            NotifyOfPropertyChange(() => PageNumber);
            NotifyOfPropertyChange(() => HasPrev);
            NotifyOfPropertyChange(() => HasNext);
            NotifyOfPropertyChange(() => Visibility);
        }

        public void Prev()
        {
            PageIndex = PageIndex - 1;
        }

        public void Next()
        {
            PageIndex = PageIndex + 1;
        }

        private void OnPageIndexChanged(int oldPageIndex, int newPageIndex)
        {
            if (PageIndexChanged != null)
            {
                PageIndexChanged(this, new PageIndexChangeEventArgs(oldPageIndex, newPageIndex));
            }
        }
    }
}
