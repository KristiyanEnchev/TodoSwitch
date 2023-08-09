import { memo, useState, useCallback, useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import PropTypes from 'prop-types';
import { Plus } from 'lucide-react';

import Modal from '../../components/modal/Modal.jsx';
import TodoListItem from './TodoListItem.jsx';
import ColorPicker from '../../components/pickers/ColorPicker.jsx';
import CustomEmojiPicker from '../../components/pickers/CustomEmojiPicker.jsx';
import { openModal, closeModal } from '../../features/modal/modalSlice.jsx';
import DeleteConfirmation from '../../components/modal/DeleteConfirmation.jsx';
import DraggableList from '../Items/DraggableList.jsx';
import { Button } from '../../components/index.jsx';

const Sidebar = ({
  lists,
  colors,
  onSelectList,
  onCreateOrUpdateList,
  onDeleteList,
  onSaveOrderList,
}) => {
  const dispatch = useDispatch();
  const { currentColor } = useSelector((state) => state.theme);
  const isModalOpen = useSelector((state) => state.modal.isOpen);
  const [reorderedList, setReorderedList] = useState(lists);

  const [formState, setFormState] = useState({
    editingList: null,
    title: '',
    selectedEmoji: '1f4cb',
    selectedColor: '#60a5fa',
    listToDelete: null,
    isDeleteModal: false,
  });

  const resetForm = useCallback(() => {
    setFormState({
      editingList: null,
      title: '',
      selectedEmoji: '1f4cb',
      selectedColor: '#60a5fa',
      listToDelete: null,
      isDeleteModal: false,
    });
  }, []);

  const handleOpenModal = useCallback(
    (list = null) => {
      if (list) {
        setFormState((prevState) => ({
          ...prevState,
          editingList: list,
          title: list.title,
          selectedEmoji: list.icon,
          selectedColor: list.color,
          isDeleteModal: false,
        }));
      } else {
        resetForm();
      }
      dispatch(openModal('Sidebar'));
    },
    [dispatch, resetForm]
  );

  const handleDeleteList = useCallback(
    (id) => {
      setFormState((prevState) => ({
        ...prevState,
        listToDelete: id,
        isDeleteModal: true,
      }));
      dispatch(openModal('Sidebar'));
    },
    [dispatch]
  );

  const handleConfirmDelete = useCallback(() => {
    onDeleteList(formState.listToDelete);
    dispatch(closeModal());
  }, [formState.listToDelete, onDeleteList, dispatch]);

  const handleCreateOrUpdateList = useCallback(() => {
    if (formState.title.trim()) {
      const newList = {
        listId: formState.editingList ? formState.editingList.id : null,
        title: formState.title.trim(),
        tasksCount: formState.editingList
          ? formState.editingList.tasksCount
          : 0,
        completedCount: formState.editingList
          ? formState.editingList.completedCount
          : 0,
        icon: formState.selectedEmoji,
        colorCode: formState.selectedColor.code,
      };

      onCreateOrUpdateList(newList);
      dispatch(closeModal());
      resetForm();
    }
  }, [formState, onCreateOrUpdateList, dispatch, resetForm]);

  const handleTitleChange = useCallback((e) => {
    setFormState((prevState) => ({ ...prevState, title: e.target.value }));
  }, []);

  const handleCloseModal = useCallback(() => {
    dispatch(closeModal());
    resetForm();
  }, [dispatch, resetForm]);

  useEffect(() => {
    if (!isModalOpen) resetForm();
  }, [isModalOpen, resetForm]);

  const renderListItem = useCallback(
    (list) => (
      <TodoListItem
        key={list.id}
        list={list}
        onEdit={handleOpenModal}
        onDelete={handleDeleteList}
        onSelect={() => onSelectList(list.id)}
      />
    ),
    [handleOpenModal, handleDeleteList, onSelectList]
  );

  const handleReorder = useCallback((newOrder) => {
    const updatedOrder = newOrder.map((item, index) => ({
      ...item,
      orderIndex: index,
    }));
    setReorderedList(updatedOrder);
  }, []);

  return (
    <aside className="w-64 bg-gray-300 dark:bg-black p-4 h-full">
      <div className="flex flex-rol items-end">
        <h2 className="text-lg font-semibold mb-4 mr-4">Todo Lists</h2>
        <Button
          onClick={() => onSaveOrderList(reorderedList)}
          size="sm"
          className="bg-green-500 hover:bg-green-600 text-white font-semibold py-2 px-4 rounded-full shadow-lg transition-all duration-300 ease-in-out transform hover:scale-105 whitespace-nowrap mb-2"
        >
          Save Order
        </Button>
      </div>
      <DraggableList
        items={lists}
        renderItem={renderListItem}
        onReorder={handleReorder}
      />
      <button
        onClick={() => handleOpenModal()}
        style={{ backgroundColor: currentColor }}
        className="mt-4 w-full py-2 bg-blue-500 text-white rounded-lg flex items-center justify-center"
      >
        <Plus size={16} className="mr-2" /> Add New List
      </button>

      <Modal
        title={
          formState.isDeleteModal
            ? 'Confirm Deletion'
            : formState.editingList
            ? 'Edit List'
            : 'Create New List'
        }
        showConfirmButton
        onConfirm={
          formState.isDeleteModal
            ? handleConfirmDelete
            : handleCreateOrUpdateList
        }
        onClose={handleCloseModal}
        modalType={'Sidebar'}
      >
        {formState.isDeleteModal ? (
          <DeleteConfirmation
            message="Are you sure you want to delete this List? You will lose all Items in the List."
            onConfirm={handleConfirmDelete}
            onCancel={handleCloseModal}
            showButtons={false}
          />
        ) : (
          <div className="p-4">
            <input
              type="text"
              value={formState.title}
              onChange={handleTitleChange}
              placeholder="List name"
              className="w-full p-2 mb-4 border rounded"
            />
            <div className="flex flex-col items-center space-y-4">
              <div className="relative z-10">
                <CustomEmojiPicker
                  selectedEmoji={formState.selectedEmoji}
                  onEmojiSelect={(emoji) =>
                    setFormState((prevState) => ({
                      ...prevState,
                      selectedEmoji: emoji,
                    }))
                  }
                />
              </div>
              <div className="relative z-0">
                <ColorPicker
                  colors={colors}
                  selectedColor={formState.selectedColor.code}
                  onColorSelect={(color) =>
                    setFormState((prevState) => ({
                      ...prevState,
                      selectedColor: color,
                    }))
                  }
                />
              </div>
            </div>
          </div>
        )}
      </Modal>
    </aside>
  );
};

Sidebar.propTypes = {
  lists: PropTypes.array.isRequired,
  colors: PropTypes.array.isRequired,
  onSelectList: PropTypes.func.isRequired,
  onCreateOrUpdateList: PropTypes.func.isRequired,
  onDeleteList: PropTypes.func.isRequired,
  onSaveOrderList: PropTypes.func.isRequired,
};

export default memo(Sidebar);
