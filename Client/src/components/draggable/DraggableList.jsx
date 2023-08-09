import { Reorder } from 'framer-motion';
import PropTypes from 'prop-types';
import { useState, useEffect } from 'react';

function DraggableList({ items, renderItem, onReorder }) {
  const [tasks, setTasks] = useState(items);

  useEffect(() => {
    setTasks(items);
  }, [items]);

  const handleReorder = (newOrder) => {
    setTasks(newOrder);
    onReorder(newOrder);
  };

  return (
    <Reorder.Group
      axis="y"
      values={tasks}
      onReorder={handleReorder}
      className="space-y-3"
    >
      {tasks.map((item) => (
        <Reorder.Item key={item.id} value={item}>
          {renderItem(item)}
        </Reorder.Item>
      ))}
    </Reorder.Group>
  );
}

DraggableList.propTypes = {
  items: PropTypes.array.isRequired,
  renderItem: PropTypes.func.isRequired,
  onReorder: PropTypes.func.isRequired,
};

export default DraggableList;
