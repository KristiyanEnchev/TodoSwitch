import { memo, useState, useCallback } from 'react';
import PropTypes from 'prop-types';
import TodoItem from './TodoItem.jsx';
import { ChevronDown, ChevronRight } from 'lucide-react';
import Button from '../../components/ui/Button.jsx';

const CompletedTaskList = ({ tasks, onToggle, onDelete, onEdit }) => {
  const [open, setOpen] = useState(false);

  const handleToggleOpen = useCallback(() => setOpen((prev) => !prev), []);

  return (
    <div className="mt-4">
      <Button
        variant="outline"
        className="w-full justify-between"
        onClick={handleToggleOpen}
      >
        <span>Completed ({tasks.length})</span>
        {open ? (
          <ChevronDown className="h-4 w-4" />
        ) : (
          <ChevronRight className="h-4 w-4" />
        )}
      </Button>
      {open && (
        <ul className="mt-2">
          {tasks.map((task) => (
            <TodoItem
              key={task.id}
              task={task}
              onToggle={onToggle}
              onDelete={onDelete}
              onEdit={onEdit}
            />
          ))}
        </ul>
      )}
    </div>
  );
};

CompletedTaskList.propTypes = {
  tasks: PropTypes.array.isRequired,
  onToggle: PropTypes.func.isRequired,
  onDelete: PropTypes.func.isRequired,
  onEdit: PropTypes.func.isRequired,
};

export default memo(CompletedTaskList);
