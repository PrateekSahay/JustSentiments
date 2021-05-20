import React from 'react';
import { makeStyles, createStyles } from '@material-ui/core/styles';
import MaterialExpansionPanelSummary, { AccordionSummaryProps } from '@material-ui/core/AccordionSummary';
import { withStyles } from '@material-ui/core/styles';

export const useStyles = makeStyles(() =>
    createStyles({
        root: {
            width: '100%',
            borderRadius: '4px',
            padding: '0px'
        },
        heading: {
            fontSize: '16px',
            color: 'lightgray',
            fontWeight: 'bold',
            //fontFamily: 'helvetica',
            paddingTop: '8px'
        }
    })
);

export const ExpansionPanelSummary = (props: AccordionSummaryProps) => {
    const { ...args } = props;
    const ContainerStyled = withStyles({
        root: {
            maxHeight: '40px',
            background: 'lightskyblue',
            borderRadius: '4px 4px 0px 0px'
        },
        expanded: {
            minHeight: '40px !important',
            maxHeight: '48px !important'
        },
        content: {
            display: 'flex',
            justifyContent: 'space-between'
        }
    })(MaterialExpansionPanelSummary);
    return <ContainerStyled {...args}>{props.children ? props.children : null}</ContainerStyled>;
};
