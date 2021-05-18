import React from 'react';
import { createStyles, makeStyles, Theme } from '@material-ui/core/styles';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import './NavBar.scss'
import { Avatar } from '@material-ui/core';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      flexGrow: 1,
    },
    menuButton: {
      marginRight: theme.spacing(2),
    },
    title: {
      flexGrow: 1,
    },
  }),
);

const NavBar = () => {
  const classes = useStyles();

  return (
    <div className={classes.root}>
      <AppBar position="static" className = "Appbar">
        <Toolbar>
            <img width="100px" src = "https://ww2-secure.justanswer.com/static/fe/ja-logo/ja-logo-white-text.png" alt="Just Answer"/>            
          <Typography variant="h6" className={classes.title}>
          <b className = "heading">Just Sentiments</b>
          </Typography>
          <Avatar className = "avatar" alt="Remy Sharp" src="/broken-image.jpg">P </Avatar>
        </Toolbar>
      </AppBar>
    </div>
  );
}

export default NavBar;
