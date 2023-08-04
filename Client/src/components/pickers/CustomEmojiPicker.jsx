import PropTypes from 'prop-types';
import { useEffect, useState, useCallback, memo, Suspense, lazy } from 'react';
import { Emoji, EmojiStyle } from 'emoji-picker-react';
import { Theme, SuggestionMode } from 'emoji-picker-react';
import { FaRegEdit, FaRegTimesCircle, FaRegSmile } from 'react-icons/fa';
import { Spinner } from 'components/index.jsx';

const EmojiPicker = lazy(() => import('emoji-picker-react'));

const CustomEmojiPicker = memo(({ selectedEmoji, onEmojiSelect }) => {
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const [currentEmoji, setCurrentEmoji] = useState(selectedEmoji || '1fae5');

  useEffect(() => {
    setCurrentEmoji(selectedEmoji);
  }, [selectedEmoji]);

  const handleEmojiClick = useCallback(
    (emojiData) => {
      setCurrentEmoji(emojiData.unified);
      onEmojiSelect(emojiData.unified);
      setShowEmojiPicker(false);
    },
    [onEmojiSelect]
  );

  const toggleEmojiPicker = useCallback(() => {
    setShowEmojiPicker((prev) => !prev);
  }, []);

  const handleRemoveEmoji = useCallback(() => {
    setCurrentEmoji('1fae5');
  }, []);

  const getFrequentlyUsedEmojis = useCallback(() => {
    const frequentlyUsedEmojis = JSON.parse(
      localStorage.getItem('epr_suggested') || '[]'
    );
    return frequentlyUsedEmojis
      .sort((a, b) => b.count - a.count)
      .slice(0, 6)
      .map((emoji) => emoji.unified);
  }, []);

  return (
    <div className="flex flex-col items-center">
      <div className="relative">
        <div
          className="relative cursor-pointer bg-gray-200 rounded-full w-24 h-24 flex items-center justify-center"
          onClick={toggleEmojiPicker}
        >
          {currentEmoji ? (
            <Emoji
              unified={currentEmoji}
              emojiStyle={EmojiStyle.APPLE}
              size={64}
            />
          ) : (
            <FaRegSmile className="text-4xl text-gray-500" />
          )}
        </div>
        <div
          className="absolute bottom-0 right-0 bg-gray-200 rounded-full p-2 cursor-pointer"
          onClick={toggleEmojiPicker}
        >
          <FaRegEdit className="text-xl text-gray-700" />
        </div>
      </div>

      {showEmojiPicker && (
        <Suspense fallback={<Spinner />}>
          <div className="mt-4">
            <EmojiPicker
              onEmojiClick={handleEmojiClick}
              theme={Theme.AUTO}
              emojiStyle={EmojiStyle.APPLE}
              searchPlaceHolder="Search emoji"
              suggestedEmojisMode={SuggestionMode.FREQUENT}
              reactions={getFrequentlyUsedEmojis()}
            />
          </div>
        </Suspense>
      )}

      {currentEmoji && (
        <button
          className="mt-4 px-4 py-2 bg-red-500 text-white rounded hover:bg-red-700 flex items-center"
          onClick={handleRemoveEmoji}
        >
          <FaRegTimesCircle className="mr-2" /> Remove Emoji
        </button>
      )}
    </div>
  );
});

CustomEmojiPicker.displayName = 'CustomEmojiPicker';

CustomEmojiPicker.propTypes = {
  selectedEmoji: PropTypes.string,
  onEmojiSelect: PropTypes.func.isRequired,
};

export default CustomEmojiPicker;
