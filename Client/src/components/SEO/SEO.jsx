import { Helmet } from 'react-helmet-async';
import PropTypes from 'prop-types';
import { memo } from 'react';

const SEO = ({
  image = '',
  type = 'website',
  title = 'TodoSwitch',
  description = 'To do app project',
  url = 'https://kristiyan-enchev-website.web.app/',
  twitterDomain = 'https://kristiyan-enchev-website.web.app/',
  themeColor = '#232e58',
  author = 'Kristiyan Enchev',
  keywords = 'React, JavaScript, Todo App',
  robots = 'index, follow',
  locale = 'en_US',
  siteName = 'Todo Switch',
}) => {
  return (
    <Helmet>
      {/* Standard metadata tags */}
      <title>{title}</title>
      <meta name="description" content={description} />
      <meta name="theme-color" content={themeColor} />
      <meta name="author" content={author} />
      <meta name="viewport" content="width=device-width, initial-scale=1" />
      {keywords && <meta name="keywords" content={keywords} />}
      {robots && <meta name="robots" content={robots} />}

      {/* Open Graph / Facebook */}
      <meta property="og:url" content={url} />
      <meta property="og:type" content={type} />
      <meta property="og:title" content={title} />
      <meta property="og:description" content={description} />
      {image && <meta property="og:image" content={image} />}
      <meta property="og:locale" content={locale} />
      <meta property="og:site_name" content={siteName} />

      {/* Twitter */}
      <meta name="twitter:card" content="summary_large_image" />
      <meta name="twitter:creator" content={author} />
      {twitterDomain && <meta name="twitter:domain" content={twitterDomain} />}
      <meta name="twitter:url" content={url} />
      <meta name="twitter:title" content={title} />
      <meta name="twitter:description" content={description} />
      {image && <meta name="twitter:image" content={image} />}
    </Helmet>
  );
};
SEO.propTypes = {
  title: PropTypes.string,
  description: PropTypes.string,
  name: PropTypes.string,
  type: PropTypes.string,
  url: PropTypes.string,
  image: PropTypes.string,
  twitterDomain: PropTypes.string,
  themeColor: PropTypes.string,
  author: PropTypes.string,
  keywords: PropTypes.string,
  robots: PropTypes.string,
  locale: PropTypes.string,
  siteName: PropTypes.string,
};

export default memo(SEO);
