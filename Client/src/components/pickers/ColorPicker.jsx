import { memo, useCallback } from 'react';
import PropTypes from 'prop-types';

const ColorPicker = ({ colors, selectedColor, onColorSelect }) => {
  const handleColorSelect = useCallback(
    (color) => {
      onColorSelect(color);
    },
    [onColorSelect]
  );

  return (
    <div className="flex space-x-2">
      {colors.map((color) => (
        <button
          key={color.code}
          onClick={() => handleColorSelect(color)}
          className={`w-8 h-8 rounded-full border-2 ${
            selectedColor === color.code
              ? 'border-black dark:border-white 2px solid'
              : 'border-transparent 2px solid'
          }`}
          style={{
            backgroundColor: color.code,
            width: '32px',
            height: '32px',
            borderRadius: '50%',
            cursor: 'pointer',
          }}
          aria-label={`Select ${color.name}`}
        />
      ))}
    </div>
  );
};

ColorPicker.propTypes = {
  colors: PropTypes.array.isRequired,
  selectedColor: PropTypes.string,
  onColorSelect: PropTypes.func.isRequired,
};

export default memo(ColorPicker);
