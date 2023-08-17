import { memo, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import Label from '../../components/ui/Label.jsx';
import Input from '../../components/ui/Input.jsx';
import Textarea from '../../components/ui/Textarea.jsx';
import Button from '../../components/ui/Button.jsx';
import { useSelector } from 'react-redux';

const TodoItemForm = ({ onSave, initialData, handleClose, showButton }) => {
  const [title, setTitle] = useState('');
  const [note, setDescription] = useState('');
  const [priority, setPriority] = useState('');
  const { currentColor } = useSelector((state) => state.theme);

  const priorityOptions = ['None', 'Low', 'Medium', 'High'];

  useEffect(() => {
    if (initialData) {
      setTitle(initialData.title);
      setDescription(initialData.note);
      setPriority(initialData.priority);
    } else {
      setTitle('');
      setDescription('');
      setPriority('None');
    }
  }, [initialData]);

  const handleSave = () => {
    onSave({ title, note, priority });
  };

  return (
    <>
      <div className="grid gap-4 py-4">
        <div className="grid grid-cols-4 items-center gap-4">
          <Label htmlFor="title" className="text-right">
            Title
          </Label>
          <Input
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            className="col-span-3 text-black"
          />
        </div>
        <div className="grid grid-cols-4 items-center gap-4">
          <Label htmlFor="priority" className="text-right">
            Priority
          </Label>
          <select
            id="priority"
            value={priority}
            onChange={(e) => setPriority(e.target.value)}
            className="col-span-3 text-black bg-white border border-gray-300 rounded px-3 py-2"
          >
            {priorityOptions.map((option) => (
              <option key={option} value={option}>
                {option}
              </option>
            ))}
          </select>
        </div>
        <div className="grid grid-cols-4 items-center gap-4">
          <Label htmlFor="description" className="text-right">
            Description
          </Label>
          <Textarea
            id="description"
            value={note}
            onChange={(e) => setDescription(e.target.value)}
            className="col-span-3 text-black"
          />
        </div>
      </div>
      {showButton && (
        <div className="flex justify-end">
          <Button
            className="bg-red-500 text-white px-4 py-2  mx-4 rounded hover:bg-red-700"
            onClick={handleClose}
          >
            Cancel
          </Button>
          <Button
            style={{ backgroundColor: currentColor }}
            className="text-white px-4 py-2 rounded hover:opacity-80"
            onClick={handleSave}
          >
            Confirm
          </Button>{' '}
        </div>
      )}
    </>
  );
};

TodoItemForm.displayName = 'TodoItemForm';

TodoItemForm.propTypes = {
  initialData: PropTypes.object,
  onSave: PropTypes.func,
  handleClose: PropTypes.func,
  showButton: PropTypes.bool,
};

export default memo(TodoItemForm);
