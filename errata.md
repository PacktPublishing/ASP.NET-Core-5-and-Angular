# Errata and improvements
This file contain corrections to the errata or mistakes in the book.

## Page 73 - Creating a new app with the Angular CLI

The author says to use the command `ng create` but that command doesn't exist, the correct command is `ng new` and it's used throughout the book and hence the bullet point
number 2: 

Create a new Angular app using the `ng create` command

Should be:

Create a new Angular app using the `ng new` command

## Page 117 - ngOnInit (and other lifecycle hooks)

The second last and last bullet point should be one bullet point, but it's split after the word "event."
The last bullet point shouldn't exist as it's the continuation of the previous, "... and detaches the event handlers to avoid memory leaks..."

## Page 288 - Model-Driven/Reactive Forms

The property used in the template doesn't match the property defined in the component, so the line of code "**title**: new FormControl()" should be replaced with "**name**: new FormControl()"

The code block: 
```typescript
import { FormControl, FormGroup } from "@angular/forms";

class ModelFormComponent implements OnInit {
  form: FormGroup;
  
    ngOnInit() {
    this.form = new FormGroup({
      title: new FormControl()
    });
  }
}
 ```
 Should be:
 
 ```typescript
import { FormControl, FormGroup } from "@angular/forms";

class ModelFormComponent implements OnInit {
  form: FormGroup;
  
    ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl()
    });
  }
}
 ```
