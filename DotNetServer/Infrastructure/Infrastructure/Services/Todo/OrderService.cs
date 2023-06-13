namespace Infrastructure.Services.Todo
{
    using Domain.Entities;

    using Application.Interfaces.Services;

    public class OrderService : IOrderService
    {
        public void ReorderItems(List<TodoItem> items, string itemId, int newOrderIndex)
        {
            var todoLinkedList = new TodoLinkedList(items);
            todoLinkedList.MoveNode(itemId, newOrderIndex);
            todoLinkedList.ReorderByCompletion();

            var reorderedItems = todoLinkedList.ToList();

            items.Clear();
            items.AddRange(reorderedItems);
        }

        public void ReorderLists(List<TodoList> lists, string listId, int newOrderIndex)
        {
            var todoListItems = lists.Select(tl => new TodoItem { Id = tl.Id, OrderIndex = tl.OrderIndex }).ToList();

            var todoListLinkedList = new TodoLinkedList(todoListItems);
            todoListLinkedList.MoveNode(listId, newOrderIndex);
            todoListLinkedList.ReassignOrderIndices();

            var reorderedListIds = todoListLinkedList.ToList().Select(ti => ti.Id).ToList();

            lists.Sort((a, b) => reorderedListIds.IndexOf(a.Id).CompareTo(reorderedListIds.IndexOf(b.Id)));
        }

        private class TodoItemNode
        {
            public TodoItem Data { get; set; }
            public TodoItemNode Next { get; set; }

            public TodoItemNode(TodoItem data)
            {
                Data = data;
                Next = null;
            }
        }

        private class TodoLinkedList
        {
            public TodoItemNode Head { get; private set; }

            public TodoLinkedList(List<TodoItem> items)
            {
                foreach (var item in items)
                {
                    Insert(item);
                }
            }

            public void Insert(TodoItem item)
            {
                var newNode = new TodoItemNode(item);

                if (Head == null)
                {
                    Head = newNode;
                }
                else
                {
                    var current = Head;
                    while (current.Next != null)
                    {
                        current = current.Next;
                    }
                    current.Next = newNode;
                }
            }

            public void ReorderByCompletion()
            {
                if (Head == null) return;

                TodoItemNode incompleteHead = new TodoItemNode(null);
                TodoItemNode completeHead = new TodoItemNode(null);

                TodoItemNode incompleteTail = incompleteHead;
                TodoItemNode completeTail = completeHead;

                var current = Head;

                while (current != null)
                {
                    if (!current.Data.IsDone)
                    {
                        incompleteTail.Next = current;
                        incompleteTail = incompleteTail.Next;
                    }
                    else
                    {
                        completeTail.Next = current;
                        completeTail = completeTail.Next;
                    }
                    current = current.Next;
                }

                incompleteTail.Next = null;
                completeTail.Next = null;

                if (incompleteHead.Next != null)
                {
                    Head = incompleteHead.Next;
                    incompleteTail.Next = completeHead.Next;
                }
                else
                {
                    Head = completeHead.Next;
                }

                ReassignOrderIndices();
            }

            public void ReassignOrderIndices()
            {
                var current = Head;
                int index = 0;
                while (current != null)
                {
                    current.Data.OrderIndex = index++;
                    current = current.Next;
                }
            }

            public void MoveNode(string itemId, int newOrderIndex)
            {
                if (Head == null || Head.Next == null) return;

                TodoItemNode prev = null;
                TodoItemNode current = Head;

                while (current != null && current.Data.Id != itemId)
                {
                    prev = current;
                    current = current.Next;
                }

                if (current == null) return; 

                if (prev != null)
                {
                    prev.Next = current.Next;
                }
                else
                {
                    Head = current.Next;
                }

                InsertAt(newOrderIndex, current);

                ReassignOrderIndices();
            }

            private void InsertAt(int newOrderIndex, TodoItemNode node)
            {
                if (newOrderIndex == 0)
                {
                    node.Next = Head;
                    Head = node;
                    return;
                }

                var current = Head;
                for (int i = 0; i < newOrderIndex - 1 && current.Next != null; i++)
                {
                    current = current.Next;
                }

                node.Next = current.Next;
                current.Next = node;
            }

            public List<TodoItem> ToList()
            {
                var list = new List<TodoItem>();
                var current = Head;

                while (current != null)
                {
                    list.Add(current.Data);
                    current = current.Next;
                }

                return list;
            }
        }
    }
}