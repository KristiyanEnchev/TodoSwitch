import { memo, useRef, useEffect } from 'react';
import PropTypes from 'prop-types';
import { motion } from 'framer-motion';

const CircularProgress = ({ value, size = 80, strokeWidth = 8 }) => {
  const radius = (size - strokeWidth) / 2;
  const circumference = radius * 2 * Math.PI;
  const strokeDashoffset = circumference - (value / 100) * circumference;
  const progressGradientId = useRef(
    `progress-gradient-${Math.random().toString(36).substr(2, 9)}`
  );
  const bgGradientId = useRef(
    `bg-gradient-${Math.random().toString(36).substr(2, 9)}`
  );
  const textGradientId = useRef(
    `text-gradient-${Math.random().toString(36).substr(2, 9)}`
  );

  useEffect(() => {
    const progressGradient = document.getElementById(
      progressGradientId.current
    );
    const textGradient = document.getElementById(textGradientId.current);
    if (progressGradient && textGradient) {
      const animate = () => {
        const angle = (Date.now() / 20) % 360;
        progressGradient.setAttribute(
          'gradientTransform',
          `rotate(${angle} ${size / 2} ${size / 2})`
        );
        textGradient.setAttribute(
          'gradientTransform',
          `rotate(${-angle} ${size / 2} ${size / 2})`
        );
        requestAnimationFrame(animate);
      };
      animate();
    }
  }, [size]);

  return (
    <div className="relative bg-gray-600 dark:bg-gray-700 rounded-full px-1 pt-1 pb-1 inline-flex items-center justify-center">
      <motion.svg
        width={size}
        height={size}
        className="transform -rotate-90"
        initial={{ opacity: 0, scale: 0.5 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.5 }}
      >
        <defs>
          <linearGradient
            id={progressGradientId.current}
            gradientUnits="userSpaceOnUse"
          >
            <stop offset="0%" stopColor="#00FFFF" /> {/* Cyan */}
            <stop offset="100%" stopColor="#FF00FF" /> {/* Magenta */}
          </linearGradient>
          <linearGradient
            id={bgGradientId.current}
            gradientUnits="userSpaceOnUse"
          >
            <stop offset="0%" stopColor="#FFFFFF" /> {/* White */}
            <stop offset="100%" stopColor="#D3D3D3" /> {/* Light Gray */}
          </linearGradient>
          <linearGradient
            id={textGradientId.current}
            gradientUnits="userSpaceOnUse"
          >
            <stop offset="0%" stopColor="#FF00FF" /> {/* Magenta */}
            <stop offset="100%" stopColor="#00FFFF" /> {/* Cyan */}
          </linearGradient>
        </defs>
        <motion.circle
          strokeWidth={strokeWidth}
          stroke={`url(#${bgGradientId.current})`}
          fill="transparent"
          r={radius}
          cx={size / 2}
          cy={size / 2}
          initial={{ pathLength: 0 }}
          animate={{ pathLength: 1 }}
          transition={{ duration: 1, ease: 'easeInOut' }}
        />
        <motion.circle
          className="transition-all duration-300 ease-in-out"
          strokeWidth={strokeWidth}
          strokeDasharray={circumference}
          strokeDashoffset={strokeDashoffset}
          strokeLinecap="round"
          stroke={`url(#${progressGradientId.current})`}
          fill="transparent"
          r={radius}
          cx={size / 2}
          cy={size / 2}
          initial={{ pathLength: 0 }}
          animate={{ pathLength: value / 100 }}
          transition={{ duration: 1, ease: 'easeInOut' }}
        />
      </motion.svg>
      <motion.div
        className="absolute inset-0 flex items-center justify-center"
        initial={{ opacity: 0, scale: 0.5 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ delay: 0.5, duration: 0.2 }}
      >
        <span
          className="text-2xl font-bold"
          style={{ fill: `url(#${textGradientId.current})` }}
        >
          <svg width={size} height={size}>
            <text
              x="50%"
              y="50%"
              dominantBaseline="middle"
              textAnchor="middle"
              fill={`url(#${textGradientId.current})`}
              fontSize="20"
              fontWeight="bold"
            >
              {Math.round(value)}%
            </text>
          </svg>
        </span>
      </motion.div>
    </div>
  );
};

CircularProgress.displayName = 'CircularProgress';

CircularProgress.propTypes = {
  value: PropTypes.number.isRequired,
  size: PropTypes.number,
  strokeWidth: PropTypes.number,
};

export default memo(CircularProgress);
