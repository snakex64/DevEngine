using DevEngine.Core.Class;
using DevEngine.Core.Project;
using DevEngine.UI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DevEngine.UI.Controls
{
    public partial class SolutionExplorer : ComponentBase
    {
        public enum TreeViewItemType
        {
            Folder, Class
        }

        private class SolutionExplorerTreeViewItem
        {
            /// <summary>
            /// Only ever null while it's being created / before it's named the first time
            /// </summary>
            public string? Name { get; set; }

            public TreeViewItemType Type { get; set; }

            public List<SolutionExplorerTreeViewItem> Children { get; } = new List<SolutionExplorerTreeViewItem>();

            public bool IsRenaming { get; set; }
            public FakeTypes.Class.DevClass? Class { get; internal set; }
        }

        private List<SolutionExplorerTreeViewItem> Items { get; } = new List<SolutionExplorerTreeViewItem>();

        private IList<SolutionExplorerTreeViewItem> ExpandedTreeViewItems = new List<SolutionExplorerTreeViewItem>();

        private SolutionExplorerTreeViewItem? SelectedTreeViewItem { get; set; }

        [CascadingParameter]
        public RightClickController? RightClickController { get; set; }

        [Parameter]
        public EventCallback<FakeTypes.Class.DevClass> OnOpenClassRequested { get; set; }

        private void OnSelectedTreeViewItemChanged(SolutionExplorerTreeViewItem item)
        {
            SelectedTreeViewItem = item;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                InitializeTreeView();

                StateHasChanged();
            }
        }

        #region Right click menus

        #region OnCreateFolderClicked

        private void OnCreateFolderClicked()
        {
            if (SelectedTreeViewItem != null)
            {
                var item = new SolutionExplorerTreeViewItem()
                {
                    Name = "",
                    Type = TreeViewItemType.Folder,
                    IsRenaming = true
                };
                SelectedTreeViewItem.Children.Add(item);

                ExpandedTreeViewItems.Add(SelectedTreeViewItem);

                StateHasChanged();
            }
        }

        #endregion

        #region OnDeleteFolderClicked

        private void OnDeleteFolderClicked()
        {

        }

        #endregion

        #region OnCreateClassClicked

        private void OnCreateClassClicked()
        {
            if (SelectedTreeViewItem == null)
                return;

            var item = new SolutionExplorerTreeViewItem()
            {
                Type = TreeViewItemType.Class,
                IsRenaming = true,
                Name = ""
            };
            SelectedTreeViewItem.Children.Add(item);

            ExpandedTreeViewItems.Add(SelectedTreeViewItem);
            StateHasChanged();
        }

        #endregion

        #endregion

        #region OnRenameClassClicked

        private void OnRenameClassClicked()
        {

        }

        #endregion

        #region OnClassNamedAfterCreation

        private Task OnClassNamedAfterCreation(SolutionExplorerTreeViewItem item, string name)
        {
            if (item.Class != null)
                throw new Exception($"Cannot call {nameof(OnClassNamedAfterCreation)} on existing class");

            var fullPathArr = GetSolutionExplorerItemFullPath(item);
            var fullPath = string.Join(".", fullPathArr.Select(x => x.Name));

            var devClass = new FakeTypes.Class.DevClass(Program.Project, null, new DevClassName(fullPath, name), fullPath.Replace(".", "/"));
            item.Class = devClass;
            Program.Project.Classes.Add(devClass);
            item.Name = name;

            return OpenClass(devClass);
        }

        #endregion

        #region OpenClass

        private Task OpenClass(FakeTypes.Class.DevClass devClass)
        {
            return OnOpenClassRequested.InvokeAsync(devClass);
        }

        #endregion

        #region OnClick

        private void OnSolutionExplorerRightClick(MouseEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Button == 2 && SelectedTreeViewItem != null)
            {
                List<RightClickController.MenuItem> items;
                if (SelectedTreeViewItem.Type == TreeViewItemType.Folder)
                {
                    items = new List<RightClickController.MenuItem>()
                    {
                        new RightClickController.MenuItem("Create folder", OnCreateFolderClicked),
                        new RightClickController.MenuItem("Create class", OnCreateClassClicked),
                    };
                    if (SelectedTreeViewItem.Children.Count == 0 && SelectedTreeViewItem != Items[0])
                        items.Add(new RightClickController.MenuItem("Delete folder", OnDeleteFolderClicked));
                }
                else
                {
                    items = new List<RightClickController.MenuItem>()
                    {
                        new RightClickController.MenuItem("Rename", OnRenameClassClicked)
                    };
                }

                RightClickController?.DisplayRightClickMenu(mouseEventArgs, items);
            }
        }

        #endregion

        #region InitializeTreeView

        private void InitializeTreeView()
        {
            Items.Add(new SolutionExplorerTreeViewItem()
            {
                Name = "/",
                Type = TreeViewItemType.Folder
            });

            foreach (var classToAdd in Program.Project.Classes)
            {
                var treeViewItem = GetTreeViewItemFromFolder(classToAdd.Value.Folder);

                if (treeViewItem != null)
                {
                    treeViewItem.Children.Add(new SolutionExplorerTreeViewItem()
                    {
                        Name = classToAdd.Key.Name,
                        Type = TreeViewItemType.Class,
                        Class = (classToAdd.Value as FakeTypes.Class.DevClass) ?? throw new Exception("Classes in project cannot be of other type then FakeTypes.Class.DevClass")
                    });
                }
            }

            foreach (var item in Items.Where(x => x.Type == TreeViewItemType.Folder))
                ExpandedTreeViewItems.Add(item);
        }

        #endregion

        #region GetTreeViewItemFromFolder

        private SolutionExplorerTreeViewItem? GetTreeViewItemFromFolder(string folder, bool create = true)
        {
            var currentFolder = Items[0]; // we assume the top one always exist

            var subFolders = folder.Split("/");

            foreach (var subFolder in subFolders)
            {
                var subFolderItem = currentFolder.Children.FirstOrDefault(x => x.Name == subFolder);
                if (subFolderItem != null)
                    currentFolder = subFolderItem;
                else if (create)
                {
                    currentFolder = new SolutionExplorerTreeViewItem()
                    {
                        Name = subFolder,
                        Type = TreeViewItemType.Folder
                    };
                    currentFolder.Children.Add(currentFolder);
                }
                else
                    return null;
            }

            return currentFolder;

        }

        #endregion

        #region OnTreeViewItemRenamed

        private async Task OnTreeViewItemRenamed(SolutionExplorerTreeViewItem item, string newValue)
        {
            item.IsRenaming = false;

            if (string.IsNullOrWhiteSpace(newValue))
                return;

            if (item.Type == TreeViewItemType.Folder)
            {
                item.Name = newValue;


                var allClasses = GetAllClassesInFolder(item);
                foreach (var subClass in allClasses)
                {
                    var fullPath = GetSolutionExplorerItemFullPath(subClass);

                    if (subClass.Class is DevEngine.FakeTypes.Class.DevClass devClass)
                        devClass.Folder = string.Join("/", fullPath.Select(x => x.Name));
                }
            }
            else if (item.Type == TreeViewItemType.Class && item.Class == null) // first time we rename a class
            {
                item.IsRenaming = false;
                await OnClassNamedAfterCreation(item, newValue);
            }
            else
                throw new NotImplementedException("Cannot rename classes from the SolutionExplorer yet");
        }

        #endregion

        #region GetAllClassesInFolder

        private IEnumerable<SolutionExplorerTreeViewItem> GetAllClassesInFolder(SolutionExplorerTreeViewItem folder)
        {
            if (folder.Type != TreeViewItemType.Folder)
                throw new ArgumentException("provided SolutionExplorerTreeViewItem must be a folder");

            foreach (var subFolder in folder.Children)
            {
                if (subFolder.Type == TreeViewItemType.Class)
                    yield return subFolder ?? throw new Exception("Class shoudln't be null here");
                else if (subFolder.Type == TreeViewItemType.Folder)
                {
                    var subClasses = GetAllClassesInFolder(subFolder);
                    foreach (var subClass in subClasses)
                        yield return subClass;
                }
            }
        }

        #endregion

        #region GetSolutionExplorerItemFullPath

        private IEnumerable<SolutionExplorerTreeViewItem> GetSolutionExplorerItemFullPath(SolutionExplorerTreeViewItem itemToFind)
        {

            foreach (var item in Items)
            {
                if (TryGetSolutionItemPathInAllChildren(item, itemToFind, out var fullPath))
                {
                    yield return item;

                    foreach (var subItem in fullPath)
                        yield return subItem;

                    yield break;
                }
            }
        }

        #endregion

        #region GetSolutionItemPathInAllChildren

        private bool TryGetSolutionItemPathInAllChildren(SolutionExplorerTreeViewItem parent, SolutionExplorerTreeViewItem child, [MaybeNullWhen(false)] out List<SolutionExplorerTreeViewItem> subItems)
        {
            if (parent.Type == TreeViewItemType.Class)
            {
                subItems = null;
                return false;
            }

            foreach (var currentChild in parent.Children)
            {
                if (currentChild == child)
                {
                    subItems = new List<SolutionExplorerTreeViewItem> { currentChild };
                    return true;
                }

                if (TryGetSolutionItemPathInAllChildren(currentChild, child, out var subSubItems))
                {
                    subSubItems.Insert(0, parent);
                    subItems = subSubItems;
                    return true;
                }
            }

            subItems = null;
            return false;
        }

        #endregion
    }

}
