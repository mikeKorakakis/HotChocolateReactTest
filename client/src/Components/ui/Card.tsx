import { Card, CardMedia, CardContent, Typography, CardActions, Button, makeStyles } from "@material-ui/core";

export interface CardProps {
    firstName: string;
    lastName: string;
    email: string;
}

const useStyles = makeStyles((theme) => ({
    cardGrid: {
      paddingTop: theme.spacing(8),
      paddingBottom: theme.spacing(8),
    },
    card: {
      height: '100%',
      width:"400px",
      display: 'flex',
      flexDirection: 'column',
    },
    cardMedia: {
      paddingTop: '56.25%', // 16:9
    },
    cardContent: {
      flexGrow: 1,
    }
  }));

const CustomCard: React.FC<CardProps> = ({firstName, lastName, email}) => {
    const classes = useStyles();
	return (
		<Card className={classes.card}>
			<CardMedia
				className={classes.cardMedia}
				image="https://source.unsplash.com/random"
				title="Image title"
			/>
			<CardContent className={classes.cardContent}>
				<Typography gutterBottom variant="h5" component="h2">
					User
				</Typography>
				<Typography>
					{firstName}{" "}{lastName}<br></br>				
					{email}
				</Typography>
			</CardContent>
		
		</Card>
	);
};

export default CustomCard;
