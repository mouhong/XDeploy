using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.Workspace.Shared.ViewModels
{
    public class PagerViewModel : PropertyChangedBase
    {
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
                    _pageIndex = value;
                    NotifyOfPropertyChange(() => PageIndex);
                    NotifyOfPropertyChange(() => HasPrev);
                    NotifyOfPropertyChange(() => HasNext);
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

        public IPagerHost Host { get; private set; }

        public PagerViewModel(IPagerHost host, int pageSize)
        {
            Host = host;
            PageSize = pageSize;
        }

        public void Update<T>(PagedQueryable<T> query, int pageIndex)
        {
            TotalRecords = query.Count;
            PageIndex = pageIndex;

            NotifyOfPropertyChange(() => TotalPages);
            NotifyOfPropertyChange(() => PageNumber);
            NotifyOfPropertyChange(() => HasPrev);
            NotifyOfPropertyChange(() => HasNext);
            NotifyOfPropertyChange(() => Visibility);
        }

        protected IEnumerable<IResult> Prev()
        {
            return Host.LoadPage(PageIndex - 1);
        }

        protected IEnumerable<IResult> Next()
        {
            return Host.LoadPage(PageIndex + 1);
        }
    }
}
