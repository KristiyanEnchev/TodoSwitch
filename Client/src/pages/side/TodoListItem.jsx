import { memo, useCallback } from 'react';
import PropTypes from 'prop-types';
import { Emoji, EmojiStyle } from 'emoji-picker-react';
import { useState } from 'react';
import { MdDelete, MdEdit } from 'react-icons/md';
import { motion } from 'framer-motion';

const TodoListItem = ({ list, onEdit, onDelete, onSelect }) => {
  const [isHovered, setIsHovered] = useState(false);

  const handleMouseEnter = useCallback(() => setIsHovered(true), []);
  const handleMouseLeave = useCallback(() => setIsHovered(false), []);
  const handleSelectClick = useCallback(
    () => onSelect(list.id),
    [onSelect, list.id]
  );
  const handleEditClick = useCallback(
    (e) => {
      e.stopPropagation();
      onEdit(list);
    },
    [onEdit, list]
  );
  const handleDeleteClick = useCallback(
    (e) => {
      e.stopPropagation();
      onDelete(list.id);
    },
    [onDelete, list.id]
  );

  return (
    <motion.div
      className="relative"
      initial={{ opacity: 0, y: -20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
    >
      {' '}
      <div
        className="mb-2 rounded-lg overflow-hidden cursor-pointer"
        style={{ backgroundColor: list.color.code }}
        onMouseEnter={handleMouseEnter}
        onMouseLeave={handleMouseLeave}
        onClick={handleSelectClick}
      >
        <div className="flex items-center justify-between p-3">
          <div className="flex items-center space-x-2">
            <Emoji
              unified={list.icon}
              emojiStyle={EmojiStyle.APPLE}
              size={24}
            />
            <span className="font-bold text-gray-800">{list.title}</span>
          </div>
          <div className="flex items-center space-x-2">
            {isHovered && (
              <>
                <button
                  onClick={handleEditClick}
                  className="text-white hover:text-gray-200"
                >
                  <MdEdit className="text-green-800 " size={20} />
                </button>
                <button
                  onClick={handleDeleteClick}
                  className="text-white hover:text-gray-200"
                >
                  <MdDelete className="text-rose-500" size={20} />
                </button>
              </>
            )}
            <span className="text-sm text-black">
              {list.completedCount}/{list.taskCounts}
            </span>
          </div>
        </div>
      </div>
    </motion.div>
  );
};

TodoListItem.propTypes = {
  list: PropTypes.object.isRequired,
  onEdit: PropTypes.func.isRequired,
  onDelete: PropTypes.func.isRequired,
  onSelect: PropTypes.func.isRequired,
};

export default memo(TodoListItem);
